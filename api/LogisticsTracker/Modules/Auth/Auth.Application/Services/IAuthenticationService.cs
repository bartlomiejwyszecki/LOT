using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;

namespace Auth.Application.Services;

public interface IAuthenticationService
{
    Task<Guid> RegisterAsync(RegisterRequestDto request);
    Task SendVerificationCodeAsync(Guid userId, string email);
    Task VerifyEmailAsync(VerifyEmailRequestDto request);
    Task SelectRoleAsync(SelectRoleRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task LogoutAsync(Guid userId);
    Task<UserProfileDto> GetCurrentUserAsync(Guid userId);
    Task ChangePasswordAsync(ChangePasswordRequestDto request);
    Task RequestPasswordResetAsync(string email);
    Task ResetPasswordAsync(PasswordResetRequestDto request);
}