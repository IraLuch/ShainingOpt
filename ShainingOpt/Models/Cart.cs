using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; }

        public int? UserId { get; set; }

        [Display(Name = "Пользователь")]
        public User? User { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
