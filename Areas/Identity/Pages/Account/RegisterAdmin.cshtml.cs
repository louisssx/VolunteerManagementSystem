using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VolunteerManagementSystem.Areas.Identity.Pages.Account
{
    // Handles the admin registration logic for the application
    public class RegisterAdminModel : PageModel
    {
        // UserManager for creating and managing users
        private readonly UserManager<IdentityUser> _userManager;

        // SignInManager for handling sign-in operations
        private readonly SignInManager<IdentityUser> _signInManager;

        // RoleManager for managing user roles
        private readonly RoleManager<IdentityRole> _roleManager;

        // The required code for admin registration
        private const string AdminRegistrationCode = "ADMIN2024";

        // Constructor injects UserManager, SignInManager, and RoleManager
        public RegisterAdminModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // Holds the input data from the registration form
        [BindProperty]
        public InputModel Input { get; set; }

        // Model for registration form input fields
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string AdminCode { get; set; }
        }

        // Handles GET requests to the registration page
        public void OnGet() { }

        // Handles POST requests for registration form submission
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate the model
            if (!ModelState.IsValid)
                return Page();

            // Check if the admin code matches the required code
            if (Input.AdminCode != AdminRegistrationCode)
            {
                ModelState.AddModelError("Input.AdminCode", "Invalid admin registration code.");
                return Page();
            }

            // Create a new user with the provided email and password
            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                // Ensure the Admin role exists, create if not
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));

                // Add the new user to the Admin role
                await _userManager.AddToRoleAsync(user, "Admin");

                // Simulate email confirmation by generating a confirmation link
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var userId = user.Id;
                var confirmUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);
                TempData["ConfirmationLink"] = confirmUrl;
                TempData["ShowConfirmationModal"] = true;

                // Redirect to the registration page with confirmation modal
                return RedirectToPage("/Account/RegisterAdmin", new { confirmed = "false" });
            }
            // Add errors to the model state if registration failed
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }
    }
} 