namespace Auth.Domain.Exceptions;

public class InvalidPasswordResetTokenException : AuthDomainException
{
    public InvalidPasswordResetTokenException(string message)
        : base(message)
    {
    }

    public InvalidPasswordResetTokenException()
        : base("Password reset token is invalid or has expired.")
    {
    }
}