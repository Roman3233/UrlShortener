using Moq;
using UrlShortener.Core.Entities;
using UrlShortener.Core.Exceptions;
using UrlShortener.Core.Interfaces;
using UrlShortener.Core.Services;

namespace UrlShortener.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private AuthService CreateSut() =>
        new(_userRepository.Object, _passwordHasher.Object);

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsUser()
    {
        var user = new User
        {
            Id = 1,
            Login = "john",
            PasswordHash = "hashed_secret",
            Role = UserRole.User
        };

        _userRepository.Setup(r => r.GetByLoginAsync("john"))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.VerifyPassword("hashed_secret", "secret123"))
            .Returns(true);

        var result = await CreateSut().AuthenticateAsync("john", "secret123");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("john", result.Login);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
    {
        var user = new User
        {
            Id = 1,
            Login = "john",
            PasswordHash = "hashed_secret",
            Role = UserRole.User
        };

        _userRepository.Setup(r => r.GetByLoginAsync("john"))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.VerifyPassword("hashed_secret", "wrong_pass"))
            .Returns(false);

        var result = await CreateSut().AuthenticateAsync("john", "wrong_pass");

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
    {
        _userRepository.Setup(r => r.GetByLoginAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        var result = await CreateSut().AuthenticateAsync("nonexistent", "secret123");

        Assert.Null(result);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("   ", "password")]
    [InlineData("john", "")]
    [InlineData("john", "   ")]
    public async Task AuthenticateAsync_EmptyCredentials_ReturnsNull(string login, string password)
    {
        var result = await CreateSut().AuthenticateAsync(login, password);

        Assert.Null(result);
        _userRepository.Verify(r => r.GetByLoginAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ValidData_CreatesAndReturnsUser()
    {
        _userRepository.Setup(r => r.ExistsByLoginAsync("newuser"))
            .ReturnsAsync(false);
        _passwordHasher.Setup(h => h.HashPassword("secret123"))
            .Returns("hashed_secret123");

        User? capturedUser = null;
        _userRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .Returns(Task.CompletedTask);

        var result = await CreateSut().RegisterAsync("newuser", "secret123", UserRole.Admin);

        Assert.NotNull(result);
        Assert.Same(capturedUser, result);
        Assert.Equal("newuser", result.Login);
        Assert.Equal("hashed_secret123", result.PasswordHash);
        Assert.Equal(UserRole.Admin, result.Role);

        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateLogin_ThrowsUserAlreadyExistsException()
    {
        _userRepository.Setup(r => r.ExistsByLoginAsync("existinguser"))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<UserAlreadyExistsException>(() =>
            CreateSut().RegisterAsync("existinguser", "secret123"));

        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _userRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("   ", "password")]
    [InlineData("user", "")]
    [InlineData("user", "   ")]
    public async Task RegisterAsync_EmptyInputs_ThrowsArgumentException(string login, string password)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            CreateSut().RegisterAsync(login, password));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUserFromRepository()
    {
        var user = new User { Id = 5, Login = "alice" };
        _userRepository.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(user);

        var result = await CreateSut().GetByIdAsync(5);

        Assert.Same(user, result);
    }
}
