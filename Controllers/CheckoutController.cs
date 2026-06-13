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
            var userId = HttpContext.Session.GetString("UserId") ?? "guest";
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();
            
                return RedirectToAction("Index", "Cart");

            ViewBag.Total = cartItems.Sum(x => x.UnitPrice * x.Quantity);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string Email, string PhoneNumber, string ShippingAddress)
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "guest";
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();

                return RedirectToAction("Index", "Cart");

            var order = new Order
            {
                UserId = userId,
                Email = Email,
                PhoneNumber = PhoneNumber,
                ShippingAddress = ShippingAddress,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = cartItems.Sum(x => x.UnitPrice * x.Quantity)
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

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}