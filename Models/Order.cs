using System.ComponentModel.DataAnnotations;

namespace NutShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

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

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
