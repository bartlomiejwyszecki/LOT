namespace Auth.Domain.Exceptions;

public class InvalidEmailVerificationTokenException : AuthDomainException
{
    public InvalidEmailVerificationTokenException(string message)
        : base(message)
    {
    }

    public InvalidEmailVerificationTokenException()
        : base("Email verification token is invalid or has expired.")
    {
    }
}