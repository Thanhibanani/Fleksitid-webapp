using Fleksitid.Data;
using Fleksitid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fleksitid.Controllers
{
    [Authorize]
    public class TidsregistreringController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;


        public TidsregistreringController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var entries = await _db.TimeEntries
                .Where(entries => entries.UserId == userId)
                .OrderByDescending(entries => entries.Date)
                .ToListAsync();

            return View(entries);
        }

        public IActionResult Create()
        {
            var entry = new TimeEntry { Date = DateTime.Today };
            return View(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TimeEntry entry)
        {
            entry.UserId = _userManager.GetUserId(User)!;

            _db.TimeEntries.Add(entry);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}