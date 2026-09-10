using UrlShortener.Core.Entities;

namespace UrlShortener.Core.Interfaces;

public interface IShortUrlRepository
{
    Task<ShortUrl?> GetByIdAsync(int id);
    Task<ShortUrl?> GetByOriginalUrlAsync(string originalUrl);
    Task<ShortUrl?> GetByShortCodeAsync(string shortCode);
    Task<List<ShortUrl>> GetAllAsync();
    Task AddAsync(ShortUrl shortUrl);
    void Update(ShortUrl shortUrl);
    void Delete(ShortUrl shortUrl);
    Task SaveChangesAsync();
}
