namespace Auth.Domain.Entities;

public class User : Entity
{
    private const int EmailVerificationTokenExpiryHours = 24;
    private const int PasswordResetTokenExpiryHours = 1;

    public Email Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string? PasswordHash { get; private set; }

    public Role Role { get; private set; }

    public bool IsEmailVerified { get; private set; }

    public string? EmailVerificationToken { get; private set; }

    public DateTime? EmailVerificationTokenExpiry { get; private set; }

    public string? PasswordResetToken { get; private set; }

    public DateTime? PasswordResetTokenExpiry { get; private set; }

    public bool IsActive { get; private set; }

    private User(
        Email email,
        string firstName,
        string lastName,
        Role role)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        IsEmailVerified = false;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static User CreateLocal(
        string email,
        string firstName,
        string lastName,
        string passwordHash)
    {
        ValidateNames(firstName, lastName);

        var emailValueObject = Email.Create(email);

        var user = new User(
            emailValueObject,
            firstName,
            lastName,
            Role.User);

        user.PasswordHash = passwordHash;

        return user;
    }

    public static User CreateOAuth(
        string email,
        string firstName,
        string lastName)
    {
        ValidateNames(firstName, lastName);

        var emailValueObject = Email.Create(email);

        var user = new User(
            emailValueObject,
            firstName,
            lastName,
            Role.User)
        {
            IsEmailVerified = true
        };

        return user;
    }

    public void GenerateEmailVerificationToken(string token)
    {
        if (IsEmailVerified)
        {
            throw new EmailAlreadyVerifiedException();
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be empty.", nameof(token));
        }

        EmailVerificationToken = token;
        EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(EmailVerificationTokenExpiryHours);
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail(string token)
    {
        if (IsEmailVerified)
        {
            throw new EmailAlreadyVerifiedException();
        }

        if (!IsEmailVerificationTokenValid())
        {
            throw new InvalidEmailVerificationTokenException("Email verification token has expired.");
        }

        if (EmailVerificationToken != token)
        {
            throw new InvalidEmailVerificationTokenException("Email verification token is invalid.");
        }

        IsEmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiry = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEmailVerificationTokenValid()
    {
        return EmailVerificationToken != null
            && EmailVerificationTokenExpiry.HasValue
            && EmailVerificationTokenExpiry.Value > DateTime.UtcNow;
    }

    public void GeneratePasswordResetToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be empty.", nameof(token));
        }

        PasswordResetToken = token;
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(PasswordResetTokenExpiryHours);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsPasswordResetTokenValid()
    {
        return PasswordResetToken != null
            && PasswordResetTokenExpiry.HasValue
            && PasswordResetTokenExpiry.Value > DateTime.UtcNow;
    }

    public void ResetPassword(string token, string newPasswordHash)
    {
        if (!IsPasswordResetTokenValid())
        {
            throw new InvalidPasswordResetTokenException("Password reset token has expired.");
        }

        if (PasswordResetToken != token)
        {
            throw new InvalidPasswordResetTokenException("Password reset token is invalid.");
        }

        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
        }

        PasswordHash = passwordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(Role newRole)
    {
        if (newRole == null)
        {
            throw new ArgumentNullException(nameof(newRole));
        }

        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    private static void ValidateNames(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        }

        if (firstName.Length > 100)
        {
            throw new ArgumentException("First name is too long (max 100 characters).", nameof(firstName));
        }

        if (lastName.Length > 100)
        {
            throw new ArgumentException("Last name is too long (max 100 characters).", nameof(lastName));
        }
    }
}