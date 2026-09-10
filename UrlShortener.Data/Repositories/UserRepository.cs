using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Entities;
using UrlShortener.Core.Interfaces;

namespace UrlShortener.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetByLoginAsync(string login)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Login == login);
    }

    public Task<bool> ExistsByLoginAsync(string login)
    {
        return _context.Users.AnyAsync(u => u.Login == login);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
