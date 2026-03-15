using Auth.Domain.Entities;

namespace Auth.Application.Interfaces;
public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    (Guid? UserId, string? Email, string? Role) ValidateAccessToken(string token);
}
