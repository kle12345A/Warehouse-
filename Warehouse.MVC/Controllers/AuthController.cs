using Microsoft.AspNetCore.Mvc;

namespace Warehouse.MVC.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
