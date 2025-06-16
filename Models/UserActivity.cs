namespace VolunteerManagementSystem.Models
{
    // Base class to track who created or modified an entity and when
    public class UserActivity
    {
        public string? createdById { get; set; } // ID of the user who created the record

        public DateTime createdOn { get; set; } // Timestamp when the record was created

        public string? modifiedById { get; set; } // ID of the user who last modified the record

        public DateTime modifiedOn { get; set; } // Timestamp of the last modification
    }
}
