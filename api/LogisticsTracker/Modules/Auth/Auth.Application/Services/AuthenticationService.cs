using System.Security.Cryptography;
using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions;
using Auth.Domain.ValueObjects;

namespace Auth.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private static readonly Role[] SelectableRoles =
    [
        Role.Merchant,
        Role.Recipient,
        Role.Carrier,
        Role.Courier
    ];

    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailService _emailService;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHashingService passwordHashingService,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _emailService = emailService;
    }

    public async Task<Guid> RegisterAsync(RegisterRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _userRepository.ExistsAsync(request.Email))
        {
            throw new UserAlreadyExistsException(request.Email);
        }

        Password.ValidateOrThrow(request.Password);

        var passwordHash = _passwordHashingService.HashPassword(request.Password);
        var user = User.CreateLocal(
            request.Email,
            request.FirstName,
            request.LastName,
            passwordHash);

        await _userRepository.AddAsync(user);

        return user.Id;
    }

    public async Task SendVerificationCodeAsync(Guid userId, string email)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new AuthDomainException("User was not found.");

        if (!string.Equals(user.Email.Value, email?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new AuthDomainException("Verification email does not match the user account.");
        }

        var code = GenerateVerificationCode();
        user.GenerateEmailVerificationToken(code);

        await _userRepository.UpdateAsync(user);
        await _emailService.SendVerificationCodeAsync(user.Email.Value, code);
    }

    public async Task VerifyEmailAsync(VerifyEmailRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new AuthDomainException("User was not found.");

        user.VerifyEmail(request.Code);

        await _userRepository.UpdateAsync(user);
    }

    public async Task SelectRoleAsync(SelectRoleRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new AuthDomainException("User was not found.");

        if (!user.IsEmailVerified)
        {
            throw new AuthDomainException("Email must be verified before selecting a role.");
        }

        if (!Enum.TryParse<Role>(request.Role, true, out var selectedRole))
        {
            throw new UnauthorizedRoleChangeException("Selected role is not supported.");
        }

        if (!SelectableRoles.Contains(selectedRole))
        {
            throw new UnauthorizedRoleChangeException(
                "Only Merchant, Recipient, Carrier, and Courier roles can be selected by the user.");
        }

        user.ChangeRole(selectedRole);

        await _userRepository.UpdateAsync(user);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new AuthDomainException("Invalid email or password.");

        if (!user.IsActive)
        {
            throw new AuthDomainException("User account is inactive.");
        }

        if (!user.IsEmailVerified)
        {
            throw new AuthDomainException("Email address has not been verified.");
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash)
            || !_passwordHashingService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new AuthDomainException("Invalid email or password.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email.Value, user.Role.ToString());
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.Add(RefreshTokenLifetime);

        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = MapToUserProfile(user)
        };
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new AuthDomainException("User was not found.");

        if (!user.IsActive)
        {
            throw new AuthDomainException("User account is inactive.");
        }

        var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

        if (!isValid)
        {
            throw new AuthDomainException("Refresh token is invalid or expired.");
        }

        await _refreshTokenService.InvalidateRefreshTokenAsync(request.UserId);

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email.Value, user.Role.ToString());
        var newRefreshToken = GenerateRefreshToken();
        var newRefreshTokenExpiry = DateTime.UtcNow.Add(RefreshTokenLifetime);

        await _refreshTokenService.StoreRefreshTokenAsync(user.Id, newRefreshToken, newRefreshTokenExpiry);

        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public Task LogoutAsync(Guid userId)
    {
        return _refreshTokenService.InvalidateRefreshTokenAsync(userId);
    }

    public async Task<UserProfileDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new AuthDomainException("User was not found.");

        return MapToUserProfile(user);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new AuthDomainException("User was not found.");

        if (string.IsNullOrWhiteSpace(user.PasswordHash)
            || !_passwordHashingService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new AuthDomainException("Current password is invalid.");
        }

        Password.ValidateOrThrow(request.NewPassword);

        var newPasswordHash = _passwordHashingService.HashPassword(request.NewPassword);
        user.SetPasswordHash(newPasswordHash);

        await _userRepository.UpdateAsync(user);
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            return;
        }

        var code = GenerateVerificationCode();
        user.GeneratePasswordResetToken(code);

        await _userRepository.UpdateAsync(user);
        await _emailService.SendPasswordResetCodeAsync(user.Email.Value, code);
    }

    public async Task ResetPasswordAsync(PasswordResetRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new AuthDomainException("User was not found.");

        Password.ValidateOrThrow(request.NewPassword);

        var newPasswordHash = _passwordHashingService.HashPassword(request.NewPassword);
        user.ResetPassword(request.Code, newPasswordHash);

        await _userRepository.UpdateAsync(user);
    }

    private static string GenerateVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    private static UserProfileDto MapToUserProfile(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email.Value,
            FullName = user.GetFullName(),
            Role = user.Role.ToString(),
            IsEmailVerified = user.IsEmailVerified
        };
    }
}