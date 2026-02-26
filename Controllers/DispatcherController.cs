using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHLManagementSystem.Controllers
{
    [Authorize(Roles = "Dispatcher")]
    public class DispatcherController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}