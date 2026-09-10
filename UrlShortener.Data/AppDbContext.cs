using Microsoft.EntityFrameworkCore;
using UrlShortener.Data.Entities;

namespace UrlShortener.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.Login).IsRequired().HasMaxLength(64);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasConversion<int>();
        });

        builder.Entity<ShortUrl>(entity =>
        {
            entity.HasIndex(e => e.OriginalUrl).IsUnique();
            entity.HasIndex(e => e.ShortCode).IsUnique();

            entity.Property(e => e.OriginalUrl).IsRequired();
            entity.Property(e => e.ShortCode).IsRequired().HasMaxLength(16);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany(u => u.ShortUrls)
                  .HasForeignKey(e => e.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}