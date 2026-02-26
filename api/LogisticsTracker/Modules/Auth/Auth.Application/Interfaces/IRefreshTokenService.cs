namespace Auth.Application.Interfaces;

public interface IRefreshTokenService
{
    Task StoreRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryDate);
    Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
    Task InvalidateRefreshTokenAsync(Guid userId);
}