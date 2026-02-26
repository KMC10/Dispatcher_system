using System.ComponentModel.DataAnnotations;

namespace DHLManagementSystem.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        public string? PlateNumber { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public string? Status { get; set; } // Available, Busy, Maintenance
    }
}