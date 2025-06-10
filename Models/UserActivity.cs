namespace VolunteerManagementSystem.Models
{
    public class UserActivity
    {
        public string? createdById { get; set; }
        public DateTime createdOn { get; set; }

        public string? modifiedById { get; set; }
        public DateTime modifiedOn { get; set; }
    }
}
