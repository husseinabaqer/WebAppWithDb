using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWithDb.Data;
using WebAppWithDb.Data.Tables;
using System.Linq;

namespace WebAppWithDb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminDriversController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminDriversController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        { _db = db; _userManager = userManager; }

        // GET: /Admin/AdminDrivers/Search?name=&phone=&car=
        public async Task<IActionResult> Search(string? name, string? phone, string? car)
        {
            // 1) start simple: base query
            IQueryable<Driver> q = _db.Drivers;

            // 2) filters (lowercase both sides)
            if (!string.IsNullOrWhiteSpace(name))
            {
                var n = name.Trim().ToLower();
                q = q.Where(d => (d.FullName ?? "").ToLower().Contains(n));
            }

            if (!string.IsNullOrWhiteSpace(car))
            {
                var c = car.Trim().ToLower();
                q = q.Where(d => (d.CarNumber ?? "").ToLower().Contains(c));
            }

            // 3) include AFTER filters, right before executing
            var drivers = await q.Include(d => d.CoveredCities).ToListAsync();

            // 4) attach phone numbers (from AspNetUsers)
            var results = new List<(Driver d, string phone)>();
            foreach (var d in drivers)
            {
                var u = await _userManager.FindByIdAsync(d.UserId ?? "");
                results.Add((d, u?.PhoneNumber ?? ""));
            }

            // 5) apply phone filter in-memory (keeps EF part simple)
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var p = phone.Trim().ToLower();
                results = results
                    .Where(r => (r.phone ?? "").ToLower().Contains(p))
                    .ToList();
            }

            return View(results);
        }

        // GET: /Admin/AdminDrivers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var d = await _db.Drivers
                .Include(x => x.CoveredCities)
                .Include(x => x.Requests).ThenInclude(r => r.Student)
                .SingleOrDefaultAsync(x => x.DriverId == id);

            if (d == null) return NotFound();

            var user = await _userManager.FindByIdAsync(d.UserId ?? "");
            ViewBag.Phone = user?.PhoneNumber ?? "";
            ViewBag.Pending = d.Requests.Where(r => r.Status == "Pending").ToList();
            ViewBag.Accepted = d.Requests.Where(r => r.Status == "Accepted").ToList();
            ViewBag.Rejected = d.Requests.Where(r => r.Status == "Rejected" || r.Status == "RemovedByDriver").ToList();

            return View(d);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(int id)
        {
            var d = await _db.Drivers.FindAsync(id);
            if (d == null) return NotFound();

            d.IsActive = false;
            d.AvailableSeats = 0; // hide from search immediately
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Offer disabled.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var d = await _db.Drivers.FindAsync(id);
            if (d == null) return NotFound();

            d.IsActive = true;
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Offer reactivated.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
