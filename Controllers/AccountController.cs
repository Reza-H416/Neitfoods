using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutShop.Data;
using NutShop.Models;
using NutShop.Services;

namespace NutShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AccountController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered";
                return View();
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = _authService.HashPassword(password),
                RegisteredAt = DateTime.UtcNow,
                IsAdmin = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !_authService.VerifyPassword(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            if (user.IsAdmin)
                HttpContext.Session.SetString("IsAdmin", "true");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(int id, string fullName, string phoneNumber, string shippingAddress)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != id)
                return Unauthorized();

            var user = await _context.Users.FindAsync(id);
            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.ShippingAddress = shippingAddress;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Profile");
        }
    }
}
