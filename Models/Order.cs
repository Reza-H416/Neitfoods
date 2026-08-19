using System.ComponentModel.DataAnnotations;

namespace NutShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        // Used for guest checkout
        public string? CartUserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string TrackingNumber { get; set; } = string.Empty;

        public DateTime? EstimatedDelivery { get; set; }

        // PAYMENT
        public string PaymentStatus { get; set; } = "Pending";

        public string? SumUpCheckoutId { get; set; }

        public string? SumUpCheckoutReference { get; set; }

        public string? SumUpTransactionCode { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}