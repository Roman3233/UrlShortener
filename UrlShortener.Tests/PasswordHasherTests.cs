using UrlShortener.Core.Services;

namespace UrlShortener.Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_ReturnsFormattedStringWithIterationsSaltAndHash()
    {
        var password = "MySecurePassword123!";
        var hash = _hasher.HashPassword(password);

        Assert.NotNull(hash);
        var parts = hash.Split(':');
        Assert.Equal(3, parts.Length);
        Assert.Equal("100000", parts[0]);
        Assert.NotEmpty(parts[1]);
        Assert.NotEmpty(parts[2]);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "MySecurePassword123!";
        var hash = _hasher.HashPassword(password);

        var result = _hasher.VerifyPassword(hash, password);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var password = "MySecurePassword123!";
        var hash = _hasher.HashPassword(password);

        var result = _hasher.VerifyPassword(hash, "WrongPassword");

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void HashPassword_EmptyPassword_ThrowsArgumentException(string password)
    {
        Assert.Throws<ArgumentException>(() => _hasher.HashPassword(password));
    }

    [Theory]
    [InlineData("invalid_format", "pass")]
    [InlineData("not:a:number:format", "pass")]
    [InlineData("", "pass")]
    [InlineData("100000:salt", "pass")]
    public void VerifyPassword_MalformedHash_ReturnsFalse(string hash, string password)
    {
        var result = _hasher.VerifyPassword(hash, password);

        Assert.False(result);
    }
}
