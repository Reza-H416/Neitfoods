using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutShop.Data;
using NutShop.Models;

namespace NutShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        private const string AdminSessionKey = "AdminLoggedIn";

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        private bool IsAdminLoggedIn()
            => HttpContext.Session.GetString(AdminSessionKey) == "true";

        // GET /Admin/Login
        public IActionResult Login(string? returnUrl = null)
        {
            if (IsAdminLoggedIn())
                return RedirectToAction("Dashboard");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, string? returnUrl = null)
        {
            var adminEmail    = _configuration["Admin:Email"]    ?? string.Empty;
            var adminPassword = _configuration["Admin:Password"] ?? string.Empty;

            if (string.Equals(email, adminEmail, StringComparison.OrdinalIgnoreCase)
                && password == adminPassword)
            {
                HttpContext.Session.SetString(AdminSessionKey, "true");
                return Redirect(returnUrl ?? "/Admin/Dashboard");
            }

            ViewBag.Error     = "Invalid email or password.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST /Admin/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(AdminSessionKey);
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View();
        }

        public async Task<IActionResult> Products()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product, IFormFile? imageFile)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await imageFile.CopyToAsync(fileStream);

                product.ImageUrl = "/images/products/" + uniqueFileName;
            }

            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Products");
        }

        // GET /Admin/EditProduct/5
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // POST /Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product updated, IFormFile? imageFile)
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = updated.Name;
            product.Description = updated.Description;
            product.Price = updated.Price;
            product.StockQuantity = updated.StockQuantity;
            product.CategoryId = updated.CategoryId;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await imageFile.CopyToAsync(fileStream);

                product.ImageUrl = "/images/products/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        public IActionResult Orders()
        {
            if (!IsAdminLoggedIn()) return RedirectToAction("Login");
            return View();
        }
    }
}
