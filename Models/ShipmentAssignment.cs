using System.ComponentModel.DataAnnotations;

namespace DHLManagementSystem.Models
{
    public class ShipmentAssignment
    {
        public int Id { get; set; }

        [Required]
        public int ShipmentId { get; set; }
        public Shipment? Shipment { get; set; }

        [Required]
        public int TripId { get; set; }
        public Trip? Trip { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}