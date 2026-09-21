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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var entry = await _db.TimeEntries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entry is not null)
            {
                _db.TimeEntries.Remove(entry);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClockIn()
        {
            var userId = _userManager.GetUserId(User);

            var alleredeInne = await _db.TimeEntries
                .AnyAsync(e => e.UserId == userId && e.EndTime == null && e.StartTime != null);

            if (!alleredeInne)
            {
                _db.TimeEntries.Add(new TimeEntry
                {
                    UserId = userId!,
                    Date = DateTime.Today,
                    StartTime = DateTime.Now
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClockOut()
        {
            var userId = _userManager.GetUserId(User);

            var apenRegistrering = await _db.TimeEntries
                .Where(e => e.UserId == userId && e.EndTime == null && e.StartTime != null)
                .FirstOrDefaultAsync();

            if (apenRegistrering is not null)
            {
                apenRegistrering.EndTime = DateTime.Now;
                apenRegistrering.Hours = (decimal)(apenRegistrering.EndTime.Value - apenRegistrering.StartTime!.Value).TotalHours;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}