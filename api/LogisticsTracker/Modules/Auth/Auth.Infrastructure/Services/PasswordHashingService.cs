using Auth.Application.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace Auth.Infrastructure.Services;

public class PasswordHashingService : IPasswordHashingService
{
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        return BC.HashPassword(password, workFactor: WorkFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ArgumentException("Hash cannot be empty.", nameof(hash));
        }

        try
        {
            return BC.Verify(password, hash);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}