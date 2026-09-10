namespace UrlShortener.Core.Entities;

public class ShortUrl
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = default!;
    public string ShortCode { get; set; } = default!;
    public DateTime CreatedDate { get; set; }

    public int CreatedByUserId { get; set; }
    public User CreatedBy { get; set; } = default!;
}
