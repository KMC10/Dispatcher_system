using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Data;
using DHLManagementSystem.Models;

namespace DHLManagementSystem.Controllers
{
    [Authorize] // restrict access (you can keep or refine this)
    public class ShipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var shipments = await _context.Shipments.ToListAsync();
            return View(shipments);
        }

        // =========================
        // CREATE (GET)
        // =========================
        [Authorize(Roles = "Dispatcher")]
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dispatcher")]
        public async Task<IActionResult> Create(Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                // Fix DateTime issue (VERY IMPORTANT)
                shipment.DeliveryDeadline = DateTime.SpecifyKind(
                    shipment.DeliveryDeadline,
                    DateTimeKind.Utc
                );

                // Default status
                shipment.Status = "Pending";

                _context.Shipments.Add(shipment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(shipment);
        }

        // =========================
        // ASSIGN SHIPMENT TO TRIP
        // =========================
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

        [HttpGet]
        public async Task<IActionResult> Recommend(int id)
        {
            // 1️⃣ Get the shipment
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null) return NotFound();

            // 2️⃣ Find trips on the same route with enough remaining capacity
            var trips = await _context.Trips
                .Include(t => t.TransportRoute)
                .Where(t => t.TransportRoute.Origin == shipment.Origin
                         && t.TransportRoute.Destination == shipment.Destination
                         && t.RemainingCapacity >= shipment.Weight
                         && t.Status == "Scheduled") // Only consider scheduled trips
                .OrderBy(t => t.DepartureTime) // Soonest departure first
                .ToListAsync();

            if (!trips.Any())
            {
                TempData["RecommendationMessage"] = "No suitable trips available for this shipment.";
                return RedirectToAction("Index", "Home");
            }

            // 3️⃣ Select the best trip (earliest departure)
            var bestTrip = trips.First();

            // 4️⃣ Assign shipment
            shipment.Status = "Assigned";
            bestTrip.RemainingCapacity -= (int)shipment.Weight;

            await _context.SaveChangesAsync();

            TempData["RecommendationMessage"] = $"Shipment assigned to trip {bestTrip.Id} departing at {bestTrip.DepartureTime:dd MMM HH:mm}";

            return RedirectToAction("Index", "Home");
        }
    }
}