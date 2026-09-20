using Fleksitid.Data;
using Fleksitid.Models;
using Fleksitid.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Fleksitid.Controllers
{
    [Authorize]
    public class ColleaguesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User)!;

            var myShares = await db.ColleagueConsents
                .Include(c => c.Viewer)
                .Where(c => c.OwnerId == userId && c.RevokedAt == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var sharedWithMe = await db.ColleagueConsents
                .Include(c => c.Owner)
                .Where(c => c.ViewerId == userId && c.RevokedAt == null && c.AcceptedAt != null)
                .OrderBy(c => c.Owner!.FullName)
                .ToListAsync();

            return View(new ColleaguesViewModel { MyShares = myShares, SharedWithMe = sharedWithMe });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShare()
        {
            var userId = userManager.GetUserId(User)!;

            var consent = new ColleagueConsent
            {
                OwnerId = userId,
                ShareToken = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddHours(24),
            };
            db.ColleagueConsents.Add(consent);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(ShowShare), new { id = consent.Id });
        }

        public async Task<IActionResult> ShowShare(int id)
        {
            var userId = userManager.GetUserId(User)!;
            var consent = await db.ColleagueConsents.FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == userId);
            if (consent is null)
            {
                return NotFound();
            }

            return View(consent);
        }

        public async Task<IActionResult> Qr(string token)
        {
            var exists = await db.ColleagueConsents.AnyAsync(c => c.ShareToken == token);
            if (!exists)
            {
                return NotFound();
            }

            var acceptUrl = Url.Action(nameof(Accept), "Colleagues", new { token }, Request.Scheme)!;

            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(acceptUrl, QRCodeGenerator.ECCLevel.Q);
            var png = new PngByteQRCode(data).GetGraphic(10);

            return File(png, "image/png");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeShare(int id)
        {
            var userId = userManager.GetUserId(User)!;
            var consent = await db.ColleagueConsents.FirstOrDefaultAsync(c =>
                c.Id == id && (c.OwnerId == userId || c.ViewerId == userId));

            if (consent is not null)
            {
                consent.RevokedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                TempData["Message"] = "Tilgangen ble fjernet.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Accept(string token)
        {
            var consent = await db.ColleagueConsents
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.ShareToken == token);

            if (consent is null || !consent.IsPending)
            {
                return View("AcceptError");
            }

            var viewer = await userManager.GetUserAsync(User);
            if (consent.OwnerId == viewer!.Id)
            {
                ViewBag.Error = "Du kan ikke godta din egen delingskode.";
                return View("AcceptError");
            }

            if (consent.Owner!.OrganizationId != viewer.OrganizationId)
            {
                ViewBag.Error = "Du kan bare se timene til kolleger i samme virksomhet.";
                return View("AcceptError");
            }

            return View(consent);
        }

        [HttpPost]
        [ActionName(nameof(Accept))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptConfirmed(string token)
        {
            var consent = await db.ColleagueConsents.Include(c => c.Owner).FirstOrDefaultAsync(c => c.ShareToken == token);
            var viewer = await userManager.GetUserAsync(User);
            var userId = viewer!.Id;

            if (consent is null || !consent.IsPending || consent.OwnerId == userId || consent.Owner!.OrganizationId != viewer.OrganizationId)
            {
                return View("AcceptError");
            }

            consent.ViewerId = userId;
            consent.AcceptedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            TempData["Message"] = "Du kan nå se timene til kollegaen din.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewColleague(string ownerId)
        {
            var userId = userManager.GetUserId(User)!;

            var hasAccess = await db.ColleagueConsents.AnyAsync(c =>
                c.OwnerId == ownerId && c.ViewerId == userId && c.AcceptedAt != null && c.RevokedAt == null);

            if (!hasAccess)
            {
                return Forbid();
            }

            var owner = await db.Users.FirstOrDefaultAsync(u => u.Id == ownerId);
            if (owner is null)
            {
                return NotFound();
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var startOfWeek = today.AddDays(-((int)today.DayOfWeek + 6) % 7);

            var entries = await db.TimeEntries.Where(e => e.UserId == ownerId)
                .OrderByDescending(e => e.Date).ToListAsync();
            var sickDays = await db.SickDays.Where(s => s.UserId == ownerId)
                .OrderByDescending(s => s.FromDate).ToListAsync();
            var contracts = await db.WorkContracts.Where(c => c.UserId == ownerId).ToListAsync();
            var activeContract = contracts.Where(c => c.IsActiveOn(today)).OrderByDescending(c => c.StartDate).FirstOrDefault();

            var vm = new ColleagueTimeViewModel
            {
                ColleagueName = owner.FullName,
                WeeklyContractHours = activeContract?.WeeklyHours ?? 0m,
                WeekWorkedHours = entries.Where(e => e.Date >= startOfWeek && e.Date <= today).Sum(e => e.Hours),
                TotalFlexBalance = FlexTimeCalculator.CalculateBalance(contracts, entries, sickDays, today),
                RecentEntries = entries.Take(20).ToList(),
                RecentSickDays = sickDays.Take(10).ToList(),
            };

            return View(vm);
        }
    }
}
