using Microsoft.AspNetCore.Mvc;

namespace VolunteerManagementSystem.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 