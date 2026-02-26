using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHLManagementSystem.Models
{
    public class Trip
    {
        public int Id { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        // Foreign Key → Vehicle
        [Required]
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        // Foreign Key → TransportRoute
        [Required]
        public int TransportRouteId { get; set; }
        public TransportRoute? TransportRoute { get; set; }

        public int RemainingCapacity { get; set; }

        public string? Status { get; set; } // Scheduled, InTransit, Completed
    }
}