using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Models;



namespace DHLManagementSystem.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext
    {
        public DbSet<ShipmentAssignment> ShipmentAssignments { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TransportRoute> Routes { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
    }
}