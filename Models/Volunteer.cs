namespace VolunteerManagementSystem.Models
{
    // Represents a volunteer user in the system
    public class Volunteer : UserActivity
    {
        public int Id { get; set; } // Unique identifier for the volunteer

        public string firstName { get; set; } // Volunteer’s first name

        public string middleName { get; set; } // Volunteer’s middle name

        public string lastName { get; set; } // Volunteer’s last name

        public string fullname => $"{firstName} {middleName} {lastName}"; // Computed full name

        public string phoneNumber { get; set; } // Contact number

        public string email { get; set; } // Email address

        public string address { get; set; } // Residential address

        public string volunteerType { get; set; } // Type/category of volunteer (e.g., Medical, Logistics)

        public string volunteerSkills { get; set; } // Skills or expertise the volunteer offers

        public bool IsApproved { get; set; } = false; // Indicates if the volunteer has been approved

        public string Status { get; set; } = "Pending"; // Registration status: Pending, Approved, or Rejected
    }
}

