using System.ComponentModel.DataAnnotations;

namespace DHLManagementSystem.Models
{
    public class TransportRoute
    {
        public int Id { get; set; }

        public string? Origin { get; set; }

        public string? Destination { get; set; }

        public decimal DistanceKm { get; set; }

        public decimal BasePrice { get; set; }
    }
}