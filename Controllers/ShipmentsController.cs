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
    public class ShipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shipments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Shipments.ToListAsync());
        }

        // GET: Shipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // GET: Shipments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Origin,Destination,Weight,DeliveryDeadline,Status")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                shipment.Status = "Registered";
                _context.Add(shipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shipment);
        }
        [HttpPost]
public async Task<IActionResult> UpdateStatus(int id, string newStatus)
{
    var shipment = await _context.Shipments.FindAsync(id);

    if (shipment == null)
        return NotFound();

    // Allowed transitions
    var validTransitions = new Dictionary<string, string[]>
    {
        { "Registered", new[] { "Evaluated" } },
        { "Evaluated", new[] { "Assigned" } },
        { "Assigned", new[] { "In Transit" } },
        { "In Transit", new[] { "Delivered" } }
    };

    if (validTransitions.ContainsKey(shipment.Status) &&
        validTransitions[shipment.Status].Contains(newStatus))
    {
        shipment.Status = newStatus;
        await _context.SaveChangesAsync();
    }

    return RedirectToAction(nameof(Index));
}

        // GET: Shipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }
            return View(shipment);
        }

        // POST: Shipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Origin,Destination,Weight,DeliveryDeadline,Status")] Shipment shipment)
        {
            if (id != shipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipmentExists(shipment.Id))
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
            return View(shipment);
        }

        // GET: Shipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipmentExists(int id)
        {
            return _context.Shipments.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Recommend(int id)
{
    var shipment = await _context.Shipments.FindAsync(id);

    if (shipment == null)
        return NotFound();

    var recommendedTrips = _context.Trips
        .Include(t => t.Vehicle)
        .Include(t => t.TransportRoute)
        .Where(t =>
          t.TransportRoute != null &&
            t.TransportRoute.Origin == shipment.Origin &&
            t.TransportRoute.Destination == shipment.Destination &&
            t.RemainingCapacity >= shipment.Weight &&
            t.Status == "Scheduled" &&
            t.DepartureTime <= shipment.DeliveryDeadline
        )
        .ToList();

    ViewBag.Shipment = shipment;
    return View(recommendedTrips);
}
    }
}
