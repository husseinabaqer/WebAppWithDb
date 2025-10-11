using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWithDb.Data;
using WebAppWithDb.Data.Tables;
using WebAppWithDb.Models;

namespace WebAppWithDb.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public RequestsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        { _db = db; _userManager = userManager; }

        private string MyUserId() => _userManager.GetUserId(User)!;

        // -------- Student: send a request after phone call --------
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int driverId)
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.UserId == MyUserId());
            if (student == null) return Forbid();

            var driver = await _db.Drivers.SingleOrDefaultAsync(d => d.DriverId == driverId);
            if (driver == null) return NotFound();

            if (driver.AvailableSeats <= 0)
            {
                TempData["Toast"] = "This offer is full.";
                return RedirectToAction("Index", "Search");
            }

            // so that no duplicate in pending/accepted
            var dup = await _db.Requests.AnyAsync(r =>
                r.DriverId == driverId && r.StudentId == student.StudentId &&
                (r.Status == "Pending" || r.Status == "Accepted"));
            if (dup)
            {
                TempData["Toast"] = "You already requested this ride.";
                return RedirectToAction("Index", "Search");
            }

            // optional: enforce gender policy
            if (driver.GenderPolicy == "MaleOnly" && string.Equals(student.Gender, "Female", StringComparison.OrdinalIgnoreCase) ||
                driver.GenderPolicy == "FemaleOnly" && string.Equals(student.Gender, "Male", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Toast"] = "This driver's gender policy doesn't match.";
                return RedirectToAction("Index", "Search");
            }

            _db.Requests.Add(new Request
            {
                DriverId = driverId,
                StudentId = student.StudentId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Request sent. The driver will approve after your call.";
            return RedirectToAction("Index", "Search");
        }

        // -------- Driver: inbox --------
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Inbox()
        {
            var driver = await _db.Drivers.SingleOrDefaultAsync(d => d.UserId == MyUserId());
            if (driver == null) return Forbid();

            var reqs = await _db.Requests.Include(r => r.Student)
                .Where(r => r.DriverId == driver.DriverId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var vm = new RequestsInboxVM
            {
                DriverId = driver.DriverId,
                Pending = reqs.Where(r => r.Status == "Pending").ToList(),
                Accepted = reqs.Where(r => r.Status == "Accepted").ToList(),
                Rejected = reqs.Where(r => r.Status == "Rejected" || r.Status == "RemovedByDriver").ToList()
            };
            return View(vm);
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var driver = await _db.Drivers.SingleOrDefaultAsync(d => d.UserId == MyUserId());
            if (driver == null) return Forbid();

            var req = await _db.Requests.Include(r => r.Student)
                .SingleOrDefaultAsync(r => r.RequestId == id && r.DriverId == driver.DriverId);
            if (req == null) return NotFound();
            if (req.Status != "Pending")
            {
                TempData["Toast"] = "Only pending requests can be accepted.";
                return RedirectToAction(nameof(Inbox));
            }
            if (driver.AvailableSeats <= 0)
            {
                TempData["Toast"] = "No seats left.";
                return RedirectToAction(nameof(Inbox));
            }

            req.Status = "Accepted";
            req.DecidedAt = DateTime.UtcNow;
            driver.AvailableSeats -= 1;

            await _db.SaveChangesAsync();
            TempData["Toast"] = $"Accepted {req.Student?.FullName}.";
            return RedirectToAction(nameof(Inbox));
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var driver = await _db.Drivers.SingleOrDefaultAsync(d => d.UserId == MyUserId());
            if (driver == null) return Forbid();

            var req = await _db.Requests.SingleOrDefaultAsync(r => r.RequestId == id && r.DriverId == driver.DriverId);
            if (req == null) return NotFound();
            if (req.Status != "Pending")
            {
                TempData["Toast"] = "Only pending requests can be rejected.";
                return RedirectToAction(nameof(Inbox));
            }

            req.Status = "Rejected";
            req.DecidedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Rejected.";
            return RedirectToAction(nameof(Inbox));
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var driver = await _db.Drivers.SingleOrDefaultAsync(d => d.UserId == MyUserId());
            if (driver == null) return Forbid();

            var req = await _db.Requests.SingleOrDefaultAsync(r => r.RequestId == id && r.DriverId == driver.DriverId);
            if (req == null) return NotFound();
            if (req.Status != "Accepted")
            {
                TempData["Toast"] = "Only accepted students can be removed.";
                return RedirectToAction(nameof(Inbox));
            }

            req.Status = "RemovedByDriver";
            req.DecidedAt = DateTime.UtcNow;
            driver.AvailableSeats += 1;

            await _db.SaveChangesAsync();
            TempData["Toast"] = "Student removed; seat reopened.";
            return RedirectToAction(nameof(Inbox));
        }
    }
}
