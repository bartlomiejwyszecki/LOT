namespace Auth.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }

    public RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public bool IsValid()
    {
        return !IsRevoked && DateTime.UtcNow <= ExpiresAt;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}