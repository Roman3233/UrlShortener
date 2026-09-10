using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Interfaces;
using UrlShortener.Core.Entities;

namespace UrlShortener.Data.Repositories;

public class ShortUrlRepository : IShortUrlRepository
{
    private readonly AppDbContext _context;

    public ShortUrlRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<ShortUrl?> GetByIdAsync(int id)
    {
        return _context.ShortUrls
            .Include(shortUrl => shortUrl.CreatedBy)
            .FirstOrDefaultAsync(shortUrl => shortUrl.Id == id);
    }

    public Task<ShortUrl?> GetByOriginalUrlAsync(string originalUrl)
    {
        return _context.ShortUrls
            .Include(shortUrl => shortUrl.CreatedBy)
            .FirstOrDefaultAsync(shortUrl => shortUrl.OriginalUrl == originalUrl);
    }

    public Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
    {
        return _context.ShortUrls
            .Include(shortUrl => shortUrl.CreatedBy)
            .FirstOrDefaultAsync(shortUrl => shortUrl.ShortCode == shortCode);
    }

    public Task<List<ShortUrl>> GetAllAsync()
    {
        return _context.ShortUrls
            .Include(shortUrl => shortUrl.CreatedBy)
            .OrderByDescending(shortUrl => shortUrl.CreatedDate)
            .ToListAsync();
    }

    public async Task AddAsync(ShortUrl shortUrl)
    {
        await _context.ShortUrls.AddAsync(shortUrl);
    }

    public void Update(ShortUrl shortUrl)
    {
        _context.ShortUrls.Update(shortUrl);
    }

    public void Delete(ShortUrl shortUrl)
    {
        _context.ShortUrls.Remove(shortUrl);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
