namespace Auth.Domain.Exceptions;

public class UnauthorizedRoleChangeException : AuthDomainException
{
    public UnauthorizedRoleChangeException()
        : base("Only administrators can change user roles.")
    {
    }

    public UnauthorizedRoleChangeException(string message) : base(message)
    {
    }
}