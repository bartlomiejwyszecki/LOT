using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using System.Collections.Concurrent;

namespace Auth.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private static readonly ConcurrentDictionary<Guid, RefreshToken> _tokens = new();

    public Task StoreRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryDate)
    {
        var token = new RefreshToken(userId, refreshToken, expiryDate);
        _tokens[userId] = token;
        return Task.CompletedTask;
    }

    public Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Task.FromResult(false);

        if (!_tokens.TryGetValue(userId, out var storedToken))
            return Task.FromResult(false);

        var isValid = storedToken.Token == refreshToken && storedToken.IsValid();
        return Task.FromResult(isValid);
    }

    public Task InvalidateRefreshTokenAsync(Guid userId)
    {
        if (_tokens.TryGetValue(userId, out var token))
        {
            token.Revoke();
        }
        return Task.CompletedTask;
    }
}