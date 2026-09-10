using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Core.DTOs;

public record CreateShortUrlRequest(
    [Required(ErrorMessage = "URL is required.")]
    [Url(ErrorMessage = "Please provide a valid HTTP or HTTPS URL.")]
    string OriginalUrl
);

public record ShortUrlDto(
    int Id,
    string OriginalUrl,
    string ShortCode,
    string ShortUrl,
    DateTime CreatedDate,
    int CreatedByUserId,
    string CreatedByLogin
);
