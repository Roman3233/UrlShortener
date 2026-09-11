using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Core.Entities;
using UrlShortener.Data;

namespace UrlShortener.Web.Controllers;

public class AboutController : Controller
{
    private readonly AppDbContext _context;

    public AboutController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var content = await _context.AboutContents.FirstOrDefaultAsync();

        content ??= new AboutContent
        {
            Description = "This URL shortener encodes the database record ID using Base62 encoding (a-z, A-Z, 0-9) to produce a compact, unique short code."
        };

        return View(content);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string description)
    {
        var content = await _context.AboutContents.FirstOrDefaultAsync();

        if (content is null)
        {
            content = new AboutContent { Description = description };
            _context.AboutContents.Add(content);
        }
        else
        {
            content.Description = description;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}