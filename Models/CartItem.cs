using System.ComponentModel.DataAnnotations;

namespace NutShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
