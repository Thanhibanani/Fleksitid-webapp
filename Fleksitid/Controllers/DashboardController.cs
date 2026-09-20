using Fleksitid.Data;
using Fleksitid.Models;
using Fleksitid.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fleksitid.Controllers
{
    [Authorize(Roles = Roles.Leder)]
    public class DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var leader = await userManager.GetUserAsync(User);
            if (leader?.OrganizationId is null)
            {
                return View(new DashboardViewModel());
            }

            var organization = await db.Organizations.FirstAsync(o => o.Id == leader.OrganizationId);
            var consultants = await db.Users
                .Where(u => u.OrganizationId == organization.Id && u.Id != leader.Id)
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var startOfWeek = today.AddDays(-((int)today.DayOfWeek + 6) % 7);
            var startOfYear = new DateOnly(today.Year, 1, 1);

            var vm = new DashboardViewModel
            {
                OrganizationName = organization.Name,
                OrgCode = organization.OrgCode,
            };

            foreach (var consultant in consultants)
            {
                var entries = await db.TimeEntries.Where(e => e.UserId == consultant.Id).ToListAsync();
                var sickDays = await db.SickDays.Where(s => s.UserId == consultant.Id).ToListAsync();
                var contracts = await db.WorkContracts.Where(c => c.UserId == consultant.Id).ToListAsync();
                var activeContract = contracts.Where(c => c.IsActiveOn(today)).OrderByDescending(c => c.StartDate).FirstOrDefault();

                vm.Consultants.Add(new ConsultantOverview
                {
                    UserId = consultant.Id,
                    FullName = consultant.FullName,
                    Email = consultant.Email ?? "",
                    WeeklyContractHours = activeContract?.WeeklyHours ?? 0m,
                    WeekWorkedHours = entries.Where(e => e.Date >= startOfWeek && e.Date <= today).Sum(e => e.Hours),
                    TotalFlexBalance = FlexTimeCalculator.CalculateBalance(contracts, entries, sickDays, today),
                    SickDaysThisYear = sickDays.Count(s => s.FromDate >= startOfYear),
                    IsClockedIn = entries.Any(e => e.IsOpen),
                });
            }

            vm.Consultants = vm.Consultants.OrderBy(c => c.FullName).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Consultant(string id)
        {
            var leader = await userManager.GetUserAsync(User);
            var consultant = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (leader?.OrganizationId is null || consultant is null || consultant.OrganizationId != leader.OrganizationId)
            {
                return NotFound();
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var startOfWeek = today.AddDays(-((int)today.DayOfWeek + 6) % 7);
            var startOfYear = new DateOnly(today.Year, 1, 1);

            var entries = await db.TimeEntries.Where(e => e.UserId == id)
                .OrderByDescending(e => e.Date).ToListAsync();
            var sickDays = await db.SickDays.Where(s => s.UserId == id)
                .OrderByDescending(s => s.FromDate).ToListAsync();
            var contracts = await db.WorkContracts.Where(c => c.UserId == id)
                .OrderByDescending(c => c.StartDate).ToListAsync();
            var activeContract = contracts.Where(c => c.IsActiveOn(today)).FirstOrDefault();

            var vm = new ConsultantDetailsViewModel
            {
                Consultant = new ConsultantOverview
                {
                    UserId = consultant.Id,
                    FullName = consultant.FullName,
                    Email = consultant.Email ?? "",
                    WeeklyContractHours = activeContract?.WeeklyHours ?? 0m,
                    WeekWorkedHours = entries.Where(e => e.Date >= startOfWeek && e.Date <= today).Sum(e => e.Hours),
                    TotalFlexBalance = FlexTimeCalculator.CalculateBalance(contracts, entries, sickDays, today),
                    SickDaysThisYear = sickDays.Count(s => s.FromDate >= startOfYear),
                    IsClockedIn = entries.Any(e => e.IsOpen),
                },
                Contracts = contracts,
                Entries = entries.Take(30).ToList(),
                SickDays = sickDays,
                NewContract = new ContractInput { UserId = id },
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContract(ContractInput newContract)
        {
            var leader = await userManager.GetUserAsync(User);
            var consultant = await db.Users.FirstOrDefaultAsync(u => u.Id == newContract.UserId);

            if (leader?.OrganizationId is null || consultant is null || consultant.OrganizationId != leader.OrganizationId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Ugyldig kontrakt. Sjekk verdiene og prøv igjen.";
                return RedirectToAction(nameof(Consultant), new { id = newContract.UserId });
            }

            // Én aktiv kontrakt om gangen: avslutt gjeldende kontrakt dagen før den nye starter.
            var previousActive = await db.WorkContracts
                .Where(c => c.UserId == newContract.UserId && c.EndDate == null)
                .ToListAsync();
            foreach (var previous in previousActive)
            {
                previous.EndDate = newContract.StartDate.AddDays(-1);
            }

            db.WorkContracts.Add(new WorkContract
            {
                UserId = newContract.UserId,
                WeeklyHours = newContract.WeeklyHours,
                StartDate = newContract.StartDate,
            });

            await db.SaveChangesAsync();
            TempData["Message"] = "Kontrakten ble lagret.";

            return RedirectToAction(nameof(Consultant), new { id = newContract.UserId });
        }
    }
}
