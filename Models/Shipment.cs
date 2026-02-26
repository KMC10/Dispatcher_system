using System.ComponentModel.DataAnnotations;

namespace DHLManagementSystem.Models
{
    public class Shipment
    {
        public int Id { get; set; }

        [Required]
        public string? Origin { get; set; }

        [Required]
        public string? Destination { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        public DateTime DeliveryDeadline { get; set; }

        public string Status { get; set; } = "Registered";
    }
}