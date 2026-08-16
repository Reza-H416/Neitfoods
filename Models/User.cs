using System.ComponentModel.DataAnnotations;

namespace NutShop.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(500)]
        public string ShippingAddress { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        public bool IsAdmin { get; set; } = false;

        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
