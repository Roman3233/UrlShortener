using UrlShortener.Core.Entities;

namespace UrlShortener.Core.Interfaces;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string login, string password);
    Task<User> RegisterAsync(string login, string password, UserRole role = UserRole.User);
    Task<User?> GetByIdAsync(int id);
}
