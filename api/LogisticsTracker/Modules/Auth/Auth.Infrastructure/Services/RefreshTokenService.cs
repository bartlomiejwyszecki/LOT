using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthDbContextImpl _dbContext;

    public RefreshTokenService(AuthDbContextImpl dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public string GenerateRefreshToken(Guid userId)
    {
        var token = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var refreshToken = new RefreshToken(userId, token, expiresAt);
        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();

        return token;
    }

    public Guid? ValidateRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var refreshToken = _dbContext.RefreshTokens
            .FirstOrDefault(rt => rt.Token == token);

        if (refreshToken == null || !refreshToken.IsValid())
            return null;

        return refreshToken.UserId;
    }

    public void RevokeRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        var refreshToken = _dbContext.RefreshTokens
            .FirstOrDefault(rt => rt.Token == token);

        if (refreshToken != null)
        {
            refreshToken.Revoke();
            _dbContext.SaveChanges();
        }
    }

    public void RevokeAllUserTokens(Guid userId)
    {
        var tokens = _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToList();

        foreach (var token in tokens)
        {
            token.Revoke();
        }

        if (tokens.Any())
        {
            _dbContext.SaveChanges();
        }
    }
}