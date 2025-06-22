using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VolunteerManagementSystem.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public bool Succeeded { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Account/Login", new { error = "Invalid confirmation link" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { error = "User not found" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                // Redirect to login page with success message
                return RedirectToPage("/Account/Login", new { registered = "true" });
            }
            else
            {
                // Redirect to login page with error message
                return RedirectToPage("/Account/Login", new { error = "Email confirmation failed" });
            }
        }
    }
} 