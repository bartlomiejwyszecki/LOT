namespace Auth.Domain.Exceptions;

public class WeakPasswordException : AuthDomainException
{
    public WeakPasswordException()
        : base(
            "Password does not meet security requirements. "
            + "Password must be at least 8 characters long, "
            + "contain at least one uppercase letter, "
            + "one lowercase letter, "
            + "one digit, "
            + "and one special character.")
    {
    }

    public WeakPasswordException(string message) : base(message)
    {
    }
}