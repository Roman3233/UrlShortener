using UrlShortener.Core.Entities;
using UrlShortener.Core.Exceptions;
using UrlShortener.Core.Interfaces;

namespace UrlShortener.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> AuthenticateAsync(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _userRepository.GetByLoginAsync(login.Trim());
        if (user is null)
            return null;

        var isValid = _passwordHasher.VerifyPassword(user.PasswordHash, password);
        if (!isValid)
            return null;

        return user;
    }

    public async Task<User> RegisterAsync(string login, string password, UserRole role = UserRole.User)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login cannot be empty.", nameof(login));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        var trimmedLogin = login.Trim();

        if (await _userRepository.ExistsByLoginAsync(trimmedLogin))
            throw new UserAlreadyExistsException(trimmedLogin);

        var user = new User
        {
            Login = trimmedLogin,
            PasswordHash = _passwordHasher.HashPassword(password),
            Role = role
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _userRepository.GetByIdAsync(id);
    }
}
