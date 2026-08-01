using Microsoft.AspNetCore.Mvc;

namespace NutShop.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
