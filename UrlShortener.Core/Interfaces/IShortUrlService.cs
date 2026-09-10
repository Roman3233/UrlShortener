using UrlShortener.Core.Entities;

namespace UrlShortener.Core.Interfaces;

public interface IShortUrlService
{
    Task<ShortUrl> CreateShortUrlAsync(string originalUrl, int createdByUserId);
    Task<List<ShortUrl>> GetAllAsync();
    Task<ShortUrl?> GetByIdAsync(int id);
    Task<string?> ResolveOriginalUrlAsync(string shortCode);
    Task DeleteAsync(int id, int requestingUserId, bool isAdmin);
}