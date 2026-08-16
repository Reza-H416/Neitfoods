using System.ComponentModel.DataAnnotations;

namespace NutShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [StringLength(200)]
        public string ShippingAddress { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime? EstimatedDelivery { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
