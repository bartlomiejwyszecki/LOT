using System.Text.RegularExpressions;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public record Email(string Value)
{
    private static readonly Regex EmailPattern = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmailException("", "Email address cannot be empty.");
        }

        var trimmed = email.Trim();

        if (trimmed.Length > 256)
        {
            throw new InvalidEmailException(trimmed, "Email address is too long (max 256 characters).");
        }

        if (!EmailPattern.IsMatch(trimmed))
        {
            throw new InvalidEmailException(trimmed, "Email format is invalid.");
        }

        return new Email(trimmed);
    }

    public override string ToString() => Value;
}