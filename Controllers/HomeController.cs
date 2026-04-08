using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DHLManagementSystem.Models;
using DHLManagementSystem.Data; // ✅ Added
using Microsoft.EntityFrameworkCore;

namespace DHLManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; // ✅ Added

        public HomeController(ApplicationDbContext context) // ✅ Added
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pendingShipments = await _context.Shipments
                .Where(s => s.Status == "Pending")
                .OrderBy(s => s.DeliveryDeadline)
                .ToListAsync();

            ViewBag.PendingShipments = pendingShipments;

            ViewBag.RecommendationStatus = pendingShipments.Any() ? "Active" : "Idle";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}