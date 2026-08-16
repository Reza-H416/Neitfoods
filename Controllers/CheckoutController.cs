using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutShop.Data;
using NutShop.Models;

namespace NutShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId.ToString())
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var user = await _context.Users.FindAsync(userId);
            ViewBag.User = user;
            ViewBag.Total = cartItems.Sum(x => x.UnitPrice * x.Quantity);
            ViewBag.CartItems = cartItems;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string phoneNumber, string shippingAddress)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId);
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId.ToString())
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var order = new Order
            {
                UserId = userId.Value,
                Email = user.Email,
                PhoneNumber = phoneNumber,
                ShippingAddress = shippingAddress,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = cartItems.Sum(x => x.UnitPrice * x.Quantity),
                TrackingNumber = "TRK" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                EstimatedDelivery = DateTime.UtcNow.AddDays(5)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.UnitPrice * item.Quantity
                };
                _context.OrderItems.Add(orderItem);

                var product = item.Product;
                product.StockQuantity -= item.Quantity;
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
