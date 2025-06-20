using System.ComponentModel.DataAnnotations;

namespace VolunteerManagementSystem.Models
{
    // Represents a volunteer user in the system
    public class Volunteer : UserActivity
    {
        public int Id { get; set; } // Unique identifier for the volunteer

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string firstName { get; set; } // Volunteer's first name

        [Required(ErrorMessage = "Middle name is required")]
        [Display(Name = "Middle Name")]
        public string middleName { get; set; } // Volunteer's middle name

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string lastName { get; set; } // Volunteer's last name

        public string fullname => $"{firstName} {middleName} {lastName}"; // Computed full name

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 digits")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must contain exactly 11 digits")]
        public string phoneNumber { get; set; } // Contact number

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com)$", ErrorMessage = "Email must end with @gmail.com or @yahoo.com")]
        public string email { get; set; } // Email address

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string address { get; set; } // Residential address

        [Required(ErrorMessage = "Volunteer type is required")]
        [Display(Name = "Volunteer Type")]
        public string volunteerType { get; set; } // Type/category of volunteer (e.g., Medical, Logistics)

        [Required(ErrorMessage = "Volunteer skills are required")]
        [Display(Name = "Volunteer Skills")]
        public string volunteerSkills { get; set; } // Skills or expertise the volunteer offers

        public bool IsApproved { get; set; } = false; // Indicates if the volunteer has been approved

        public string Status { get; set; } = "Pending"; // Registration status: Pending, Approved, or Rejected
    }
}

