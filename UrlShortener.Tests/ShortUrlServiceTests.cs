using Moq;
using UrlShortener.Core.Entities;
using UrlShortener.Core.Exceptions;
using UrlShortener.Core.Interfaces;
using UrlShortener.Core.Services;

namespace UrlShortener.Tests;

public class ShortUrlServiceTests
{
    private readonly Mock<IShortUrlRepository> _repository = new();
    private readonly Mock<IShortCodeGenerator> _generator = new();

    private ShortUrlService CreateSut() =>
        new(_repository.Object, _generator.Object);

    [Fact]
    public async Task CreateShortUrlAsync_NewUrl_CreatesAndAssignsShortCode()
    {
        ShortUrl? addedUrl = null;
        _repository
            .Setup(r => r.GetByOriginalUrlAsync("https://example.com"))
            .ReturnsAsync((ShortUrl?)null);
        _repository
            .Setup(r => r.AddAsync(It.IsAny<ShortUrl>()))
            .Callback<ShortUrl>(url =>
            {
                addedUrl = url;
                url.Id = 42;
            })
            .Returns(Task.CompletedTask);
        _generator
            .Setup(g => g.Encode(42))
            .Returns("g");

        var result = await CreateSut().CreateShortUrlAsync("https://example.com", 7);

        Assert.Same(addedUrl, result);
        Assert.Equal(42, result.Id);
        Assert.Equal("https://example.com", result.OriginalUrl);
        Assert.Equal(7, result.CreatedByUserId);
        Assert.Equal("g", result.ShortCode);
        Assert.Equal(DateTimeKind.Utc, result.CreatedDate.Kind);

        _repository.Verify(r => r.AddAsync(result), Times.Once);
        _repository.Verify(r => r.Update(result), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
        _generator.Verify(g => g.Encode(42), Times.Once);
    }

    [Fact]
    public async Task CreateShortUrlAsync_ExistingUrl_ThrowsDuplicateUrlException()
    {
        var existing = new ShortUrl { Id = 1, OriginalUrl = "https://example.com" };
        _repository
            .Setup(r => r.GetByOriginalUrlAsync(existing.OriginalUrl))
            .ReturnsAsync(existing);

        var exception = await Assert.ThrowsAsync<DuplicateUrlException>(() =>
            CreateSut().CreateShortUrlAsync(existing.OriginalUrl, 7));

        Assert.Contains(existing.OriginalUrl, exception.Message);
        _repository.Verify(r => r.AddAsync(It.IsAny<ShortUrl>()), Times.Never);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _generator.Verify(g => g.Encode(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToRepository()
    {
        var urls = new List<ShortUrl> { new() { Id = 1 }, new() { Id = 2 } };
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(urls);

        var result = await CreateSut().GetAllAsync();

        Assert.Same(urls, result);
        _repository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_DelegatesToRepository()
    {
        var url = new ShortUrl { Id = 5 };
        _repository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(url);

        var result = await CreateSut().GetByIdAsync(5);

        Assert.Same(url, result);
        _repository.Verify(r => r.GetByIdAsync(5), Times.Once);
    }

    [Fact]
    public async Task ResolveOriginalUrlAsync_ExistingCode_ReturnsOriginalUrl()
    {
        _repository
            .Setup(r => r.GetByShortCodeAsync("g"))
            .ReturnsAsync(new ShortUrl { ShortCode = "g", OriginalUrl = "https://example.com" });

        var result = await CreateSut().ResolveOriginalUrlAsync("g");

        Assert.Equal("https://example.com", result);
    }

    [Fact]
    public async Task ResolveOriginalUrlAsync_UnknownCode_ReturnsNull()
    {
        _repository
            .Setup(r => r.GetByShortCodeAsync("missing"))
            .ReturnsAsync((ShortUrl?)null);

        var result = await CreateSut().ResolveOriginalUrlAsync("missing");

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_UnknownUrl_ThrowsNotFoundException()
    {
        _repository.Setup(r => r.GetByIdAsync(9)).ReturnsAsync((ShortUrl?)null);

        var exception = await Assert.ThrowsAsync<ShortUrlNotFoundException>(() =>
            CreateSut().DeleteAsync(9, 1, false));

        Assert.Contains("9", exception.Message);
        _repository.Verify(r => r.Delete(It.IsAny<ShortUrl>()), Times.Never);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_NonOwner_ThrowsForbiddenException()
    {
        var url = new ShortUrl { Id = 9, CreatedByUserId = 2 };
        _repository.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(url);

        await Assert.ThrowsAsync<ForbiddenDeleteException>(() =>
            CreateSut().DeleteAsync(9, 1, false));

        _repository.Verify(r => r.Delete(It.IsAny<ShortUrl>()), Times.Never);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Owner_DeletesAndSaves()
    {
        var url = new ShortUrl { Id = 9, CreatedByUserId = 1 };
        _repository.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(url);

        await CreateSut().DeleteAsync(9, 1, false);

        _repository.Verify(r => r.Delete(url), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_AdminCanDeleteAnyUrl()
    {
        var url = new ShortUrl { Id = 9, CreatedByUserId = 2 };
        _repository.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(url);

        await CreateSut().DeleteAsync(9, 1, true);

        _repository.Verify(r => r.Delete(url), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
