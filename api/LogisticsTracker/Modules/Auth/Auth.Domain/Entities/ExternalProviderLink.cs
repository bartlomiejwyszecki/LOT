namespace Auth.Domain.Entities;

public class ExternalProviderLink : Entity
{
    public Guid UserId { get; private set; }

    public string Provider { get; private set; }

    public string ExternalUserId { get; private set; }

    public string Email { get; private set; }

    public DateTime LinkedAt { get; private set; }

    private ExternalProviderLink(
        Guid userId,
        string provider,
        string externalUserId,
        string email)
    {
        UserId = userId;
        Provider = provider;
        ExternalUserId = externalUserId;
        Email = email;
        LinkedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ExternalProviderLink Create(
        Guid userId,
        string provider,
        string externalUserId,
        string email)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Provider name cannot be empty.", nameof(provider));
        }

        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            throw new ArgumentException("External user ID cannot be empty.", nameof(externalUserId));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }

        return new ExternalProviderLink(userId, provider, externalUserId, email);
    }

    public string GetDisplayName()
    {
        return $"{Provider} ({Email})";
    }
}