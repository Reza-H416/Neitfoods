using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutShop.Data;
using NutShop.Models;

namespace NutShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Products");
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();

            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View("Products", products);
        }

        public IActionResult Orders()
        {
            return View();
        }
    }
}