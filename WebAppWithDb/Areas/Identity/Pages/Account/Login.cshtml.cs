using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebAppWithDb.Data.Infrastructure;

namespace WebAppWithDb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly UserManager<IdentityUser> _users;
        private readonly ILogger<LoginModel> _log;

        public LoginModel(SignInManager<IdentityUser> signIn, UserManager<IdentityUser> users, ILogger<LoginModel> log)
        { _signIn = signIn; _users = users; _log = log; }

        [BindProperty] public InputModel Input { get; set; } = new();
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required, EmailAddress] public string Email { get; set; } = string.Empty;
            [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
            [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid) return Page();

            var result = await _signIn.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            _log.LogInformation("User logged in.");
            var user = await _users.FindByEmailAsync(Input.Email);
            if (user != null)
            {
                if (await _users.IsInRoleAsync(user, Roles.Driver))
                    return LocalRedirect(Url.Action("Dashboard", "Drivers")!);
                if (await _users.IsInRoleAsync(user, Roles.Student))
                    return LocalRedirect(Url.Action("Index", "Search")!);
            }
            return LocalRedirect(returnUrl);
        }
    }
}
