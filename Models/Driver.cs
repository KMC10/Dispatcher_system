using System.ComponentModel.DataAnnotations;

namespace DHLManagementSystem.Models
{
    public class Driver
    {
        public int Id { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? LicenseNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsAvailable { get; set; }
    }
}