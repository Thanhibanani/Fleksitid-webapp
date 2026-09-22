using Fleksitid.Data;
using Fleksitid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fleksitid.Controllers
{
    [Authorize]
    public class ArbeidstidController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ArbeidstidController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var perioder = await _db.Arbeidstider
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.FraDato)
                .ToListAsync();

            return View(perioder);
        }

        public IActionResult Create()
        {
            var arbeidstid = new Arbeidstid { FraDato = DateTime.Today };
            return View(arbeidstid);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Arbeidstid arbeidstid)
        {
            arbeidstid.UserId = _userManager.GetUserId(User)!;

            _db.Arbeidstider.Add(arbeidstid);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}