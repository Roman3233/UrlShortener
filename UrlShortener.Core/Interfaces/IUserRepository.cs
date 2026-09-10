using UrlShortener.Core.Entities;

namespace UrlShortener.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByLoginAsync(string login);
    Task<bool> ExistsByLoginAsync(string login);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
