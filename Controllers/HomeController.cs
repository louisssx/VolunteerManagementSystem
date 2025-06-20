using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VolunteerManagementSystem.Models;
using VolunteerManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace VolunteerManagementSystem.Controllers
{
    [Authorize] // Requires user authentication for all actions in this controller
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Logger for tracking application events
        private readonly ApplicationDbContext _context; // Database context for data access

        // Constructor that injects the logger and database context dependencies
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger; // Initialize logger
            _context = context; // Initialize database context
        }

        // Displays the home page view
        public IActionResult Index()
        {
            return View(); // Returns the default Index view
        }

        // Displays the dashboard view with key statistics and latest incidents
        public IActionResult Dashboard()
        {
            ViewBag.VolunteerCount = _context.Volunteers.Count(v => v.IsApproved); // Count of approved volunteers
            ViewBag.IncidentReportCount = _context.IncidentReports.Count(); // Total incident reports
            ViewBag.TaskAssignmentCount = _context.TaskAssignments.Count(); // Total task assignments
            ViewBag.PendingVolunteers = _context.Volunteers.Count(v => !v.IsApproved); // Count of pending volunteers
            var latestIncidents = _context.IncidentReports // Get 5 most recent incidents
                .OrderByDescending(r => r.DateReported)
                .Take(5)
                .ToList();
            ViewBag.LatestIncidents = latestIncidents; // Pass latest incidents to view
            return View(); // Returns the Dashboard view
        }

        // Displays the error page with details about the current request
        // Uses ResponseCache attribute to prevent caching of error pages
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Returns error view with request ID
        }
    }
}
