using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace WebAppWithDb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["data"] = "something";
            return View();
        }
    }
}
