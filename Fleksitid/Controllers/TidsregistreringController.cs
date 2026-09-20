using Fleksitid.Data;
using Fleksitid.Models;
using Fleksitid.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fleksitid.Controllers
{
    [Authorize]
    public class TidsregistreringController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User)!;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var startOfWeek = today.AddDays(-((int)today.DayOfWeek + 6) % 7);

            var entries = await db.TimeEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ThenByDescending(e => e.CreatedAt)
                .ToListAsync();

            var sickDays = await db.SickDays
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.FromDate)
                .ToListAsync();

            var contracts = await db.WorkContracts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var activeContract = contracts
                .Where(c => c.IsActiveOn(today))
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefault();

            var vm = new TidsregistreringViewModel
            {
                OpenEntry = entries.FirstOrDefault(e => e.IsOpen),
                WeeklyContractHours = activeContract?.WeeklyHours ?? 0m,
                WeekWorkedHours = entries.Where(e => e.Date >= startOfWeek && e.Date <= today).Sum(e => e.Hours),
                TotalFlexBalance = FlexTimeCalculator.CalculateBalance(contracts, entries, sickDays, today),
                RecentEntries = entries.Take(20).ToList(),
                RecentSickDays = sickDays.Take(10).ToList(),
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClockIn()
        {
            var userId = userManager.GetUserId(User)!;
            var today = DateOnly.FromDateTime(DateTime.Today);

            var alreadyOpen = await db.TimeEntries
                .AnyAsync(e => e.UserId == userId && e.Source == TimeEntrySource.Stempling && e.EndTime == null);

            if (!alreadyOpen)
            {
                db.TimeEntries.Add(new TimeEntry
                {
                    UserId = userId,
                    Date = today,
                    StartTime = TimeOnly.FromDateTime(DateTime.Now),
                    Source = TimeEntrySource.Stempling
                });
                await db.SaveChangesAsync();
                TempData["Message"] = "Du er nå stemplet inn.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClockOut()
        {
            var userId = userManager.GetUserId(User)!;

            var open = await db.TimeEntries
                .Where(e => e.UserId == userId && e.Source == TimeEntrySource.Stempling && e.EndTime == null)
                .OrderByDescending(e => e.Date)
                .FirstOrDefaultAsync();

            if (open is not null)
            {
                open.EndTime = TimeOnly.FromDateTime(DateTime.Now);
                await db.SaveChangesAsync();
                TempData["Message"] = "Du er nå stemplet ut.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddManual(ManualEntryInput manualEntry)
        {
            if (!ModelState.IsValid)
            {
                return await ReturnIndexWithErrors();
            }

            var userId = userManager.GetUserId(User)!;
            db.TimeEntries.Add(new TimeEntry
            {
                UserId = userId,
                Date = manualEntry.Date,
                ManualHours = manualEntry.Hours,
                Note = manualEntry.Note,
                Source = TimeEntrySource.Manuell
            });
            await db.SaveChangesAsync();
            TempData["Message"] = "Timene ble registrert.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSickDay(SickDayInput sickDayEntry)
        {
            if (sickDayEntry.ToDate < sickDayEntry.FromDate)
            {
                ModelState.AddModelError(nameof(SickDayInput.ToDate), "Til-dato kan ikke være før fra-dato.");
            }

            if (!ModelState.IsValid)
            {
                return await ReturnIndexWithErrors();
            }

            var userId = userManager.GetUserId(User)!;
            db.SickDays.Add(new SickDay
            {
                UserId = userId,
                FromDate = sickDayEntry.FromDate,
                ToDate = sickDayEntry.ToDate,
                Note = sickDayEntry.Note
            });
            await db.SaveChangesAsync();
            TempData["Message"] = "Sykefravær ble registrert.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            var userId = userManager.GetUserId(User)!;
            var entry = await db.TimeEntries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (entry is not null)
            {
                db.TimeEntries.Remove(entry);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSickDay(int id)
        {
            var userId = userManager.GetUserId(User)!;
            var sickDay = await db.SickDays.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (sickDay is not null)
            {
                db.SickDays.Remove(sickDay);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ReturnIndexWithErrors()
        {
            // Reloads the full view model so the page can redisplay validation errors.
            var userId = userManager.GetUserId(User)!;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var startOfWeek = today.AddDays(-((int)today.DayOfWeek + 6) % 7);

            var entries = await db.TimeEntries.Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date).ThenByDescending(e => e.CreatedAt).ToListAsync();
            var sickDays = await db.SickDays.Where(s => s.UserId == userId)
                .OrderByDescending(s => s.FromDate).ToListAsync();
            var contracts = await db.WorkContracts.Where(c => c.UserId == userId).ToListAsync();
            var activeContract = contracts.Where(c => c.IsActiveOn(today)).OrderByDescending(c => c.StartDate).FirstOrDefault();

            var vm = new TidsregistreringViewModel
            {
                OpenEntry = entries.FirstOrDefault(e => e.IsOpen),
                WeeklyContractHours = activeContract?.WeeklyHours ?? 0m,
                WeekWorkedHours = entries.Where(e => e.Date >= startOfWeek && e.Date <= today).Sum(e => e.Hours),
                TotalFlexBalance = FlexTimeCalculator.CalculateBalance(contracts, entries, sickDays, today),
                RecentEntries = entries.Take(20).ToList(),
                RecentSickDays = sickDays.Take(10).ToList(),
            };

            return View(nameof(Index), vm);
        }
    }
}
