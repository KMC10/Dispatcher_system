using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DHLManagementSystem.Data;
using DHLManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace DHLManagementSystem.Controllers
{
    [Authorize(Roles = "Dispatcher")]
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Trips.Include(t => t.TransportRoute).Include(t => t.Vehicle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.TransportRoute)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // GET: Trips/Create
        public IActionResult Create()
        {
            ViewData["TransportRouteId"] = new SelectList(_context.Routes, "Id", "Id");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "PlateNumber");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DepartureTime,VehicleId,TransportRouteId,RemainingCapacity,Status")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                // 🔥 FIX: Convert to UTC
                trip.DepartureTime = DateTime.SpecifyKind(trip.DepartureTime, DateTimeKind.Utc);

                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportRouteId"] = new SelectList(_context.Routes, "Id", "Id", trip.TransportRouteId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "PlateNumber", trip.VehicleId);
            return View(trip);
        }
        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            ViewData["TransportRouteId"] = new SelectList(_context.Routes, "Id", "Id", trip.TransportRouteId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "PlateNumber", trip.VehicleId);
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DepartureTime,VehicleId,TransportRouteId,RemainingCapacity,Status")] Trip trip)
        {
            if (id != trip.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔥 FIX: Convert to UTC
                    trip.DepartureTime = DateTime.SpecifyKind(trip.DepartureTime, DateTimeKind.Utc);

                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["TransportRouteId"] = new SelectList(_context.Routes, "Id", "Id", trip.TransportRouteId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "PlateNumber", trip.VehicleId);
            return View(trip);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.TransportRoute)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip != null)
            {
                _context.Trips.Remove(trip);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Assign(int shipmentId, int tripId)
        {
            var shipment = await _context.Shipments.FindAsync(shipmentId);
            var trip = await _context.Trips.FindAsync(tripId);

            if (shipment == null || trip == null)
                return NotFound();

            if (trip.RemainingCapacity < shipment.Weight)
                return BadRequest("Not enough capacity.");

            // Reduce capacity
            trip.RemainingCapacity -= (int)shipment.Weight;

            // Update shipment status
            shipment.Status = "Assigned";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.Id == id);
        }
    }
}
