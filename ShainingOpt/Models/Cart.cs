using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Display(Name = "Пользователь")]
        [Required(ErrorMessage = "Укажите пользователя")]
        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
