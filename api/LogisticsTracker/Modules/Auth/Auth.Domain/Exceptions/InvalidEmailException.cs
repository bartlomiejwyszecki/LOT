namespace Auth.Domain.Exceptions;

public class InvalidEmailException : AuthDomainException
{
    public InvalidEmailException(string email)
        : base($"The email address '{email}' is invalid.")
    {
    }

    public InvalidEmailException(string email, string message)
        : base($"The email address '{email}' is invalid: {message}")
    {
    }
}