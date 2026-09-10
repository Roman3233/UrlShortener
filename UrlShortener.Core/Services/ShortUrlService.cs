using UrlShortener.Core.Entities;
using UrlShortener.Core.Interfaces;
using UrlShortener.Core.Exceptions;

namespace UrlShortener.Core.Services;

public class ShortUrlService : IShortUrlService
{
    private readonly IShortUrlRepository _repository;
    private readonly IShortCodeGenerator _generator;

    public ShortUrlService(IShortUrlRepository repository, IShortCodeGenerator generator)
    {
        _repository = repository;
        _generator = generator;
    }

    public async Task<ShortUrl> CreateShortUrlAsync(string originalUrl, int createdByUserId)
    {
        var existing = await _repository.GetByOriginalUrlAsync(originalUrl);
        if (existing is not null)
            throw new DuplicateUrlException(originalUrl);

        var shortUrl = new ShortUrl
        {
            OriginalUrl = originalUrl,
            CreatedByUserId = createdByUserId,
            CreatedDate = DateTime.UtcNow,
            ShortCode = string.Empty
        };

        await _repository.AddAsync(shortUrl);
        await _repository.SaveChangesAsync();

        shortUrl.ShortCode = _generator.Encode(shortUrl.Id);
        _repository.Update(shortUrl);
        await _repository.SaveChangesAsync();

        return shortUrl;
    }

    public Task<List<ShortUrl>> GetAllAsync() => _repository.GetAllAsync();

    public Task<ShortUrl?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<string?> ResolveOriginalUrlAsync(string shortCode)
    {
        var shortUrl = await _repository.GetByShortCodeAsync(shortCode);
        return shortUrl?.OriginalUrl;
    }

    public async Task DeleteAsync(int id, int requestingUserId, bool isAdmin)
    {
        var shortUrl = await _repository.GetByIdAsync(id);
        if (shortUrl is null)
            throw new ShortUrlNotFoundException(id);

        if (!isAdmin && shortUrl.CreatedByUserId != requestingUserId)
            throw new ForbiddenDeleteException();

        _repository.Delete(shortUrl);
        await _repository.SaveChangesAsync();
    }
}