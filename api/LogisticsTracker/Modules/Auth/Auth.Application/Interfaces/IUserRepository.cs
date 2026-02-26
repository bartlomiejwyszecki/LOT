namespace Auth.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid userId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsAsync(string email);
}