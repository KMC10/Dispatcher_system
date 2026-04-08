using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Data;
using DHLManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DHLManagementSystem.Controllers
{
    [Authorize(Roles = "Dispatcher")]
    public class RecommendationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecommendationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recommendations
        public async Task<IActionResult> Index()
        {
            var pendingShipments = await _context.Shipments
                .Where(s => s.Status == "Pending")
                .OrderBy(s => s.DeliveryDeadline)
                .ToListAsync();

            return View(pendingShipments);
        }

        [HttpPost]
        public async Task<IActionResult> Recommend(int shipmentId)
        {
            var shipment = await _context.Shipments.FindAsync(shipmentId);
            if (shipment == null)
                return NotFound();

            var trips = await _context.Trips
                .Include(t => t.TransportRoute)
                .Where(t =>
                    t.TransportRoute.Origin == shipment.Origin &&
                    t.TransportRoute.Destination == shipment.Destination &&
                    t.RemainingCapacity >= shipment.Weight &&
                    t.Status == "Scheduled")
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();

            if (!trips.Any())
            {
                // No suitable trips
                return Json(new { success = false, message = "No suitable trips available for this shipment." });
            }

            var bestTrip = trips.First();
            shipment.Status = "Assigned";
            bestTrip.RemainingCapacity -= (int)shipment.Weight;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Shipment assigned to trip {bestTrip.Id} departing at {bestTrip.DepartureTime:dd MMM HH:mm}."
            });
        }
    }
}