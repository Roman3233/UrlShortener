using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Core.DTOs;
using UrlShortener.Core.Entities;
using UrlShortener.Core.Exceptions;
using UrlShortener.Core.Interfaces;

namespace UrlShortener.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlsController : ControllerBase
{
    private readonly IShortUrlService _service;

    public ShortUrlsController(IShortUrlService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShortUrlDto>>> GetAll()
    {
        var urls = await _service.GetAllAsync();
        return Ok(urls.Select(MapToDto).ToList());
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ShortUrlDto>> GetById(int id)
    {
        var url = await _service.GetByIdAsync(id);
        if (url is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(url));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ShortUrlDto>> Create([FromBody] CreateShortUrlRequest request)
    {
        var userId = GetCurrentUserId();

        try
        {
            var created = await _service.CreateShortUrlAsync(request.OriginalUrl, userId);
            var dto = MapToDto(created);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (DuplicateUrlException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _service.DeleteAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (ShortUrlNotFoundException)
        {
            return NotFound();
        }
        catch (ForbiddenDeleteException)
        {
            return Forbid();
        }
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idClaim!);
    }

    private ShortUrlDto MapToDto(ShortUrl url)
    {
        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        return new ShortUrlDto(
            url.Id,
            url.OriginalUrl,
            url.ShortCode,
            $"{baseUrl}/{url.ShortCode}",
            url.CreatedDate,
            url.CreatedByUserId,
            url.CreatedBy?.Login ?? User.Identity?.Name ?? string.Empty
        );
    }
}
