using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Entities;
using UrlShortener.Core.Interfaces;

namespace UrlShortener.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        // Automatically apply any pending database migrations
        await context.Database.MigrateAsync();

        // Seed initial admin and regular user if none exist
        if (!await context.Users.AnyAsync())
        {
            var admin = new User
            {
                Login = "admin",
                PasswordHash = passwordHasher.HashPassword("admin123"),
                Role = UserRole.Admin
            };

            var user = new User
            {
                Login = "user",
                PasswordHash = passwordHasher.HashPassword("user123"),
                Role = UserRole.User
            };

            await context.Users.AddRangeAsync(admin, user);
            await context.SaveChangesAsync();
        }
    }
}
