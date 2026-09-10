namespace UrlShortener.Data.Entities;

public enum UserRole
{
    User = 0,
    Admin = 1
}

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.User;

    public ICollection<ShortUrl> ShortUrls { get; set; } = new List<ShortUrl>();
}