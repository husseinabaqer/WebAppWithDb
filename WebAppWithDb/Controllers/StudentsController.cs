using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWithDb.Data;
using WebAppWithDb.Models;

namespace WebAppWithDb.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        { _db = db; _userManager = userManager; }

        private string MyUserId() => _userManager.GetUserId(User)!;

        // GET: /Students/MyRide
        public async Task<IActionResult> MyRide()
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.UserId == MyUserId());
            if (student == null) return Forbid();

            var req = await _db.Requests.Include(r => r.Driver)
                .Where(r => r.StudentId == student.StudentId && r.Status == "Accepted")
                .OrderByDescending(r => r.DecidedAt).FirstOrDefaultAsync();

            if (req == null) return View(new StudentRideVM { HasRide = false });

            var d = req.Driver!;
            return View(new StudentRideVM
            {
                HasRide = true,
                DriverName = d.FullName,
                Route = $"{d.DriverCity} → {d.Destination}",
                Car = $"{d.CarBrand} {d.CarModel} ({d.CarType})",
                GenderPolicy = d.GenderPolicy,
                SeatsLeft = d.AvailableSeats
            });
        }

        // GET: /Students/Profile
        public async Task<IActionResult> Profile()
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.UserId == MyUserId());
            if (student == null) return Forbid();
            return View(student);
        }

        // POST: /Students/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("StudentId,FullName,Gender,Stage,City,Governorate,Destination")] Data.Tables.Student m)
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.UserId == MyUserId());
            if (student == null) return Forbid();

            student.FullName = m.FullName ?? student.FullName;
            student.Gender = m.Gender;
            student.Stage = m.Stage;
            student.City = m.City;
            student.Governorate = m.Governorate;
            student.Destination = m.Destination;

            await _db.SaveChangesAsync();
            TempData["Toast"] = "Profile updated.";
            return RedirectToAction(nameof(Profile));
        }
    }
}
