using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebAppWithDb.Data;
using WebAppWithDb.Data.Infrastructure;
using WebAppWithDb.Data.Tables;

namespace WebAppWithDb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _users;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RegisterModel> _log;

        public RegisterModel(
            UserManager<IdentityUser> users,
            SignInManager<IdentityUser> signIn,
            RoleManager<IdentityRole> roles,
            ApplicationDbContext db,
            ILogger<RegisterModel> log)
        {
            _users = users; _signIn = signIn; _roles = roles; _db = db; _log = log;
        }

        [BindProperty] public InputModel Input { get; set; } = new();
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required, EmailAddress] public string Email { get; set; } = string.Empty;
            [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
            [DataType(DataType.Password), Compare(nameof(Password))] public string? ConfirmPassword { get; set; }

            [Required, Display(Name = "Register as")] public string Role { get; set; } = Roles.Student;
            [Required, MaxLength(150)] public string FullName { get; set; } = string.Empty;
            [Phone, Display(Name = "Phone number")] public string? PhoneNumber { get; set; }
        }

        public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid) return Page();

            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber };
            var create = await _users.CreateAsync(user, Input.Password);
            if (!create.Succeeded)
            {
                foreach (var e in create.Errors) ModelState.AddModelError(string.Empty, e.Description);
                return Page();
            }

            if (!await _roles.RoleExistsAsync(Input.Role))
                await _roles.CreateAsync(new IdentityRole(Input.Role));

            await _users.AddToRoleAsync(user, Input.Role);

            // from the form in the view I get the Input.Role value
            if (Input.Role == Roles.Student)
            {
                _db.Students.Add(new Student { UserId = user.Id, FullName = Input.FullName });
            }
            else // Driver
            {
                _db.Drivers.Add(new Driver
                {
                    UserId = user.Id,
                    FullName = Input.FullName,
                    GenderPolicy = "Both",
                    CarBrand = string.Empty,
                    CarType = string.Empty,
                    CarModel = string.Empty,
                    CarSeats = 0,
                    AvailableSeats = 0,
                    CarNumber = string.Empty
                });
            }
            await _db.SaveChangesAsync();

            _log.LogInformation("User created.");
            await _signIn.SignInAsync(user, isPersistent: false);

            // redirect if (( driver ))
            if (Input.Role == Roles.Driver)
                return LocalRedirect(Url.Action("Dashboard", "Drivers")!);

            // ----------  ((student))
            return LocalRedirect(Url.Action("Index", "Search")!);
        }
    }
}
