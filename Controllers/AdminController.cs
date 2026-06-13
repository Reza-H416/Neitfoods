using Microsoft.AspNetCore.Mvc;
using NutShop.Data;

namespace NutShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }
    }
}
