using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaJournal.Data.Context;
using MediaJournal.Web.Models;

namespace MediaJournal.Web.Controllers;

public class PublicController : Controller
{
    private readonly ApplicationDbContext _context;

    public PublicController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var publicMedia = await _context.Media
            .Where(m => m.IsPublic)
            .OrderByDescending(m => m.CompletedDate)
            .ToListAsync();

        var viewModel = new PublicReviewsViewModel
        {
            MediaItems = publicMedia
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var media = await _context.Media
            .FirstOrDefaultAsync(m => m.ID == id && m.IsPublic);
            
        if (media == null)
        {
            return NotFound();
        }

        var model = new MediaDetailsViewModel
        {
            Media = media,
        };

        return View(model);
    }
}