using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWithDb.Data;
using WebAppWithDb.Data.Tables;
using WebAppWithDb.Models;

namespace WebAppWithDb.Controllers
{
    [Authorize(Roles = "Driver")]
    public class DriversController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public DriversController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        { 
            _db = db;
            _userManager = userManager; 
        }

        private string MyUserId() => _userManager.GetUserId(User)!;

        private async Task<Driver?> LoadMyDriverAsync() =>
            await _db.Drivers
                .Include(d => d.CoveredCities)
                .Include(d => d.Requests).ThenInclude(r => r.Student)
                .SingleOrDefaultAsync(d => d.UserId == MyUserId());

        // GET: /Drivers/Dashboard
        // from the Areas pages, redirected here if Driver signed up or signed in >>>
        public async Task<IActionResult> Dashboard()
        {
            var d = await LoadMyDriverAsync();
            if (d == null) return RedirectToAction(nameof(Create));
            if (d.CarSeats <= 0)
            {
                TempData["Toast"] = "Please create your offer.";
                return RedirectToAction(nameof(Create));
            }

            var vm = new DriverDashboardVM
            {
                DriverId = d.DriverId,
                FullName = d.FullName,
                DriverCity = d.DriverCity,
                Destination = d.Destination,
                CarSeats = d.CarSeats,
                AvailableSeats = d.AvailableSeats,
                PendingCount = d.Requests?.Count(r => r.Status == "Pending") ?? 0,
                AcceptedCount = d.Requests?.Count(r => r.Status == "Accepted") ?? 0,
                Cities = d.CoveredCities?.Select(c => c.City).ToList() ?? new()
            };
            return View(vm);
        }

        // GET: /Drivers/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var d = await LoadMyDriverAsync();
            if (d == null) return NotFound("Driver record not found for this user.");
            if (d.CarSeats > 0)
            {
                TempData["Toast"] = "Offer is locked. Contact admin to modify.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(new DriverOfferForm { FullName = d.FullName, GenderPolicy = "Both" });
        }

        // POST: /Drivers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DriverOfferForm m)
        {
            if (!ModelState.IsValid) return View(m);

            var d = await _db.Drivers.Include(x => x.CoveredCities)
                                     .SingleOrDefaultAsync(x => x.UserId == MyUserId());
            if (d == null) return NotFound("Driver record not found.");
            if (d.CarSeats > 0)
            {
                TempData["Toast"] = "Offer already exists and is locked.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Map VM -> entity
            d.FullName = m.FullName;
            d.DriverCity = m.DriverCity;
            d.Destination = m.Destination;
            d.CarBrand = m.CarBrand;
            d.CarType = m.CarType;
            d.CarModel = m.CarModel;
            d.CarSeats = m.CarSeats;
            d.CarNumber = m.CarNumber;
            d.GenderPolicy = m.GenderPolicy;
            d.OfferDescription = m.OfferDescription;

            d.AvailableSeats = m.CarSeats; // initial seats

            if (d.CoveredCities?.Any() == true)
                _db.CoveredCities.RemoveRange(d.CoveredCities);

            var cities = (m.CoveredCitiesCsv ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase);
            foreach (var city in cities)
                _db.CoveredCities.Add(new CoveredCity { DriverId = d.DriverId, City = city });

            await _db.SaveChangesAsync();
            TempData["Toast"] = "Offer created!";
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: /Drivers/Students
        public async Task<IActionResult> Students()
        {
            var d = await LoadMyDriverAsync();
            if (d == null || d.CarSeats <= 0) return RedirectToAction(nameof(Create));

            var accepted = await _db.Requests.Include(r => r.Student)
                .Where(r => r.DriverId == d.DriverId && r.Status == "Accepted")
                .OrderBy(r => r.CreatedAt).ToListAsync();

            return View(accepted);
        }
    }
}
