using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Data;
using DHLManagementSystem.Models;

namespace DHLManagementSystem.Controllers
{
    [Authorize] // or [Authorize(Roles = "Dispatcher")] if you want role restriction
    public class ShipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Example Index method
        public async Task<IActionResult> Index()
        {
            var shipments = await _context.Shipments.ToListAsync();
            return View(shipments);
        }

        // Example Create method
        public IActionResult Create()
        {
            return View();
        }

        // Assign shipment to trip
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dispatcher")]
        public async Task<IActionResult> Assign(int shipmentId, int tripId)
        {
            var shipment = await _context.Shipments.FindAsync(shipmentId);
            var trip = await _context.Trips.FindAsync(tripId);

            if (shipment == null || trip == null)
                return NotFound();

            bool alreadyAssigned = await _context.ShipmentAssignments
                .AnyAsync(a => a.ShipmentId == shipmentId);

            if (alreadyAssigned)
                return BadRequest("Shipment already assigned.");

            if (trip.RemainingCapacity < shipment.Weight)
                return BadRequest("Not enough capacity.");

            trip.RemainingCapacity -= (int)shipment.Weight;
            shipment.Status = "Assigned";

            var assignment = new ShipmentAssignment
            {
                ShipmentId = shipmentId,
                TripId = tripId
            };

            _context.ShipmentAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Other methods...
    }
}