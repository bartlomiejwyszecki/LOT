using System.Text.RegularExpressions;
using Auth.Domain.Exceptions;

namespace Auth.Domain.ValueObjects;

public class Password
{
    private static readonly Regex PasswordPattern = new(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*\-_=+\[\]{}|;:'"",.<>?/~`\\]).{8,}$",
        RegexOptions.Compiled);

    private Password()
    {
    }

    public static bool Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return PasswordPattern.IsMatch(password);
    }

    public static void ValidateOrThrow(string password)
    {
        if (!Validate(password))
        {
            throw new WeakPasswordException();
        }
    }

    public static string GetRequirementsMessage()
    {
        return "Password must be at least 8 characters long and contain at least one uppercase letter, "
            + "one lowercase letter, one digit, and one special character (!@#$%^&*-_=+[]{}|;:'\",.<>?/~`\\).";
    }
}