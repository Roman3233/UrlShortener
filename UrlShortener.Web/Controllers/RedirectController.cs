using Microsoft.AspNetCore.Mvc;
using UrlShortener.Core.Interfaces;

namespace UrlShortener.Web.Controllers;

[ApiController]
public class RedirectController : ControllerBase
{
    private readonly IShortUrlService _service;

    public RedirectController(IShortUrlService service)
    {
        _service = service;
    }

    [HttpGet("/{shortCode}")]
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        if (string.Equals(shortCode, "about", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "About");
        }

        var originalUrl = await _service.ResolveOriginalUrlAsync(shortCode);

        if (originalUrl is null)
        {
            return NotFound();
        }

        return Redirect(originalUrl);
    }
}