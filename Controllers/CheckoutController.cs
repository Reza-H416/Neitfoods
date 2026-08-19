using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutShop.Data;
using NutShop.Models;

namespace NutShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public CheckoutController(
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        private string GetCartUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
                return userId.Value.ToString();

            var cartUserId = HttpContext.Session.GetString("CartUserId");

            if (string.IsNullOrEmpty(cartUserId))
            {
                cartUserId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartUserId", cartUserId);
            }

            return cartUserId;
        }

        public async Task<IActionResult> Index()
        {
            var cartUserId = GetCartUserId();

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == cartUserId)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
                ViewBag.User = await _context.Users.FindAsync(userId.Value);
            else
                ViewBag.User = null;

            ViewBag.Total = cartItems.Sum(x => x.UnitPrice * x.Quantity);
            ViewBag.CartItems = cartItems;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(
            string email,
            string phoneNumber,
            string shippingAddress)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cartUserId = GetCartUserId();

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == cartUserId)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(shippingAddress))
            {
                ViewBag.User = userId.HasValue
                    ? await _context.Users.FindAsync(userId.Value)
                    : null;
                ViewBag.Total = cartItems.Sum(x => x.UnitPrice * x.Quantity);
                ViewBag.CartItems = cartItems;
                ViewBag.Error = "Email, phone number and shipping address are required.";
                return View("Index");
            }

            var total = cartItems.Sum(x => x.UnitPrice * x.Quantity);

            var order = new Order
            {
                UserId = userId,
                CartUserId = cartUserId,
                Email = email.Trim(),
                PhoneNumber = phoneNumber.Trim(),
                ShippingAddress = shippingAddress.Trim(),
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                PaymentStatus = "Pending",
                TotalAmount = total,
                TrackingNumber =
                    "TRK" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                EstimatedDelivery = DateTime.UtcNow.AddDays(5)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.UnitPrice * item.Quantity
                });
            }

            await _context.SaveChangesAsync();

            var apiKey = _configuration["SumUp:ApiKey"];
            var merchantCode = _configuration["SumUp:MerchantCode"];

            if (string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(merchantCode))
            {
                order.PaymentStatus = "ConfigurationError";
                await _context.SaveChangesAsync();
                return StatusCode(500, "SumUp payment configuration is missing.");
            }

            var checkoutReference = $"ORDER-{order.Id}-{Guid.NewGuid():N}";

            var redirectUrl = Url.Action(
                "PaymentReturn",
                "Checkout",
                new { orderId = order.Id },
                Request.Scheme)!;

            var webhookUrl = Url.Action(
                "SumUpWebhook",
                "Checkout",
                null,
                Request.Scheme)!;

            var checkoutRequest = new
            {
                checkout_reference = checkoutReference,
                amount = total,
                currency = "SEK",
                merchant_code = merchantCode,
                description = $"NeitFoods order #{order.Id}",
                redirect_url = redirectUrl,
                return_url = webhookUrl,
                hosted_checkout = new
                {
                    enabled = true
                }
            };

            var json = JsonSerializer.Serialize(checkoutRequest);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            using var requestContent =
                new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://api.sumup.com/v0.1/checkouts",
                requestContent);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                order.PaymentStatus = "CheckoutFailed";
                await _context.SaveChangesAsync();

                Console.WriteLine($"SumUp error: {responseBody}");
                return StatusCode(500, "Could not start SumUp payment.");
            }

            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            var checkoutId = root.GetProperty("id").GetString();
            var hostedCheckoutUrl = root.GetProperty("hosted_checkout_url").GetString();

            if (string.IsNullOrWhiteSpace(checkoutId) ||
                string.IsNullOrWhiteSpace(hostedCheckoutUrl))
            {
                order.PaymentStatus = "CheckoutFailed";
                await _context.SaveChangesAsync();
                return StatusCode(500, "SumUp did not return a checkout URL.");
            }

            order.SumUpCheckoutId = checkoutId;
            order.SumUpCheckoutReference = checkoutReference;

            await _context.SaveChangesAsync();

            return Redirect(hostedCheckoutUrl);
        }

        public async Task<IActionResult> PaymentReturn(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            if (!CurrentVisitorCanAccessOrder(order))
                return Unauthorized();

            await VerifyAndFinalizeOrder(order);

            if (order.PaymentStatus == "Paid")
            {
                return RedirectToAction(
                    "Confirmation",
                    new { orderId = order.Id });
            }

            ViewBag.PaymentStatus = order.PaymentStatus;
            return View("PaymentPending", order);
        }

        private bool CurrentVisitorCanAccessOrder(Order order)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue &&
                order.UserId.HasValue &&
                order.UserId.Value == userId.Value)
            {
                return true;
            }

            var cartUserId = HttpContext.Session.GetString("CartUserId");

            return !string.IsNullOrWhiteSpace(cartUserId) &&
                   !string.IsNullOrWhiteSpace(order.CartUserId) &&
                   order.CartUserId == cartUserId;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SumUpWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
                return Ok();

            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (!root.TryGetProperty("event_type", out var eventTypeElement))
                    return Ok();

                var eventType = eventTypeElement.GetString();

                if (eventType != "CHECKOUT_STATUS_CHANGED")
                    return Ok();

                if (!root.TryGetProperty("id", out var checkoutIdElement))
                    return Ok();

                var checkoutId = checkoutIdElement.GetString();

                if (string.IsNullOrWhiteSpace(checkoutId))
                    return Ok();

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.SumUpCheckoutId == checkoutId);

                if (order != null)
                    await VerifyAndFinalizeOrder(order);
            }
            catch (JsonException)
            {
            }

            return Ok();
        }

        private async Task VerifyAndFinalizeOrder(Order order)
        {
            if (order.PaymentStatus == "Paid")
                return;

            if (string.IsNullOrWhiteSpace(order.SumUpCheckoutId))
                return;

            var apiKey = _configuration["SumUp:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                return;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await client.GetAsync(
                $"https://api.sumup.com/v0.1/checkouts/{order.SumUpCheckoutId}");

            if (!response.IsSuccessStatusCode)
                return;

            var responseBody = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            var status = root.GetProperty("status").GetString();

            if (status == "PAID")
            {
                if (order.PaymentStatus != "Paid")
                {
                    order.PaymentStatus = "Paid";
                    order.Status = "Paid";

                    if (root.TryGetProperty("transactions", out var transactions) &&
                        transactions.ValueKind == JsonValueKind.Array &&
                        transactions.GetArrayLength() > 0)
                    {
                        var transaction = transactions[0];

                        if (transaction.TryGetProperty(
                                "transaction_code",
                                out var transactionCode))
                        {
                            order.SumUpTransactionCode = transactionCode.GetString();
                        }
                    }

                    foreach (var item in order.OrderItems)
                    {
                        if (item.Product != null)
                        {
                            item.Product.StockQuantity -= item.Quantity;

                            if (item.Product.StockQuantity < 0)
                                item.Product.StockQuantity = 0;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(order.CartUserId))
                    {
                        var cartItems = await _context.CartItems
                            .Where(c => c.UserId == order.CartUserId)
                            .ToListAsync();

                        _context.CartItems.RemoveRange(cartItems);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            else if (status == "FAILED")
            {
                order.PaymentStatus = "Failed";
                await _context.SaveChangesAsync();
            }
            else if (status == "EXPIRED")
            {
                order.PaymentStatus = "Expired";
                await _context.SaveChangesAsync();
            }
            else
            {
                order.PaymentStatus = "Pending";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            if (!CurrentVisitorCanAccessOrder(order))
                return Unauthorized();

            if (order.PaymentStatus != "Paid")
            {
                return RedirectToAction(
                    "PaymentReturn",
                    new { orderId = order.Id });
            }

            return View(order);
        }
    }
}