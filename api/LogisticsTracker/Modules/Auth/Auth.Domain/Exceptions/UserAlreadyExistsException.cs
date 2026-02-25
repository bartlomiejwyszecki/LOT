namespace Auth.Domain.Exceptions;

public class UserAlreadyExistsException : AuthDomainException
{
    public UserAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists.")
    {
    }
}