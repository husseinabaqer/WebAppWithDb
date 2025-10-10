using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWithDb.Data;
using WebAppWithDb.Models;

namespace WebAppWithDb.Controllers
{
    [Authorize(Roles = "Student")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public SearchController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        { _db = db; _userManager = userManager; }

        // GET: /Search
        public async Task<IActionResult> Index(SearchFiltersVM f)
        {
            var q = _db.Drivers
                .Include(d => d.CoveredCities)
                .Where(d => d.AvailableSeats > 0); // hide full cars

            if (!string.IsNullOrWhiteSpace(f.City))
            {
                var city = f.City.ToLower();
                q = q.Where(d =>
                    d.CoveredCities.Any(c => c.City.ToLower().Contains(city)) ||
                    (d.DriverCity ?? "").ToLower().Contains(city));
            }

            if (!string.IsNullOrWhiteSpace(f.Destination))
            {
                var dest = f.Destination.ToLower();
                q = q.Where(d => (d.Destination ?? "").ToLower().Contains(dest));
            }

            if (!string.IsNullOrWhiteSpace(f.GenderPolicy) && f.GenderPolicy != "Both")
                q = q.Where(d => d.GenderPolicy == f.GenderPolicy);

            if (!string.IsNullOrWhiteSpace(f.CarType))
            {
                var type = f.CarType.ToLower();
                q = q.Where(d => d.CarType.ToLower().Contains(type));
            }

            var drivers = await q.ToListAsync();

            var results = new List<SearchResultItemVM>();
            foreach (var d in drivers)
            {
                var user = await _userManager.FindByIdAsync(d.UserId!); // phone
                results.Add(new SearchResultItemVM
                {
                    DriverId = d.DriverId,
                    DriverName = d.FullName ?? "",
                    Route = $"{d.DriverCity} → {d.Destination}",
                    Car = $"{d.CarBrand} {d.CarModel} ({d.CarType})",
                    GenderPolicy = d.GenderPolicy,
                    AvailableSeats = d.AvailableSeats,
                    PhoneNumber = user?.PhoneNumber ?? ""
                });
            }

            return View(new SearchResultsVM { Filters = f, Results = results });
        }
    }
}
