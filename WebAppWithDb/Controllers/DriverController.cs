using Microsoft.AspNetCore.Mvc;

namespace WebAppWithDb.Controllers
{
    public class DriverController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
