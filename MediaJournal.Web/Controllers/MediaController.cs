using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaJournal.Data.Context;
using MediaJournal.Models.Entities;
using MediaJournal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MediaJournal.Web.Controllers
{
    [Authorize]
    public class MediaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MediaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userMedia = await _context.Media
                .Where(m => m.UserId == currentUser.Id)
                .OrderByDescending(m => m.CompletedDate)
                .ToListAsync();

            var viewModel = new MediaLibraryViewModel
            {
                MediaItems = userMedia
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
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (media == null)
            {
                return NotFound();
            }

            var model = new MediaDetailsViewModel()
            {
                Media = media,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new MediaFormViewModel
            {
                Media = new Media { CompletedDate = DateTime.Today },
                Action = "Create"
            };

            return View("MediaForm", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("ID,Title,Type,CompletedDate,Rating,Review,UserId,IsPublic")] Media media)
        {
            if (ModelState.IsValid)
            {
                media.UserId = _userManager.GetUserId(User);
                _context.Add(media);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new MediaFormViewModel
            {
                Media = media
            };

            return View("MediaForm", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var media = await _context.Media.FindAsync(id);
            if (media == null)
            {
                return NotFound();
            }

            var viewModel = new MediaFormViewModel
            {
                Media = media,
                Action = "Edit"
            };

            return View("MediaForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ID,Title,Type,CompletedDate,Rating,Review,UserId,IsPublic")] Media media)
        {
            if (id != media.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    media.UserId = _userManager.GetUserId(User);
                    _context.Update(media);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MediaExists(media.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            var viewModel = new MediaFormViewModel
            {
                Media = media,
                Action = "Edit"
            };

            return View("MediaForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var media = await _context.Media
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (media == null)
            {
                return NotFound();
            }

            _context.Media.Remove(media);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var userMedia = await _context.Media
                .Where(m => m.UserId == userId)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                MediaItems = userMedia,
                TotalItems = userMedia.Count,
                ItemsByType = userMedia.GroupBy(m => m.Type)
                    .ToDictionary(g => g.Key, g => g.Count()),
                AverageRating = userMedia.Any() ? userMedia.Average(m => m.Rating) : 0,
                RatingDistribution = userMedia.GroupBy(m => m.Rating)
                    .ToDictionary(g => g.Key, g => g.Count()),
                RecentItems = userMedia.OrderByDescending(m => m.CompletedDate)
                    .Take(5)
                    .ToList(),
                MostReviewedType = userMedia.Any() ? 
                    userMedia.GroupBy(m => m.Type)
                        .OrderByDescending(g => g.Count())
                        .First().Key : MediaType.Book
            };

            return View(viewModel);
        }
        
        private bool MediaExists(int id)
        {
            return _context.Media.Any(e => e.ID == id);
        }
    }
}