using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace VolunteerManagementSystem.Areas.Identity.Pages.Account
{
    // Handles the logout logic for the application
    public class LogoutModel : PageModel
    {
        // SignInManager for handling sign-out operations
        private readonly SignInManager<IdentityUser> _signInManager;

        // Logger for logging logout events
        private readonly ILogger<LogoutModel> _logger;

        // Constructor injects SignInManager and Logger
        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // Handles POST requests to log out the user
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Sign out the user
            await _signInManager.SignOutAsync();

            // Set a logout message to display on the login page
            TempData["LogoutMessage"] = "You have been logged out successfully.";

            // Redirect to the login page
            return RedirectToPage("./Login");
        }
    }
}
