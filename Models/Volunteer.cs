using Microsoft.AspNetCore.Mvc.Rendering;

namespace VolunteerManagementSystem.Models
{
    public class Volunteer : UserActivity
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string fullname => $"{firstName} {middleName} {lastName}";
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string volunteerType{ get; set; }
        public string volunteerSkills { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}
