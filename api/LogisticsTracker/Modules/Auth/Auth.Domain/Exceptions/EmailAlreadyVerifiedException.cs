namespace Auth.Domain.Exceptions;

public class EmailAlreadyVerifiedException : AuthDomainException
{
    public EmailAlreadyVerifiedException()
        : base("Email has already been verified.")
    {
    }

    public EmailAlreadyVerifiedException(string message) : base(message)
    {
    }
}