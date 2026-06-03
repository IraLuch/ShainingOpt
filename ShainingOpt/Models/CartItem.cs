using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShainingOpt.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Display(Name = "Корзина")]
        [Required(ErrorMessage ="Укажите корзину")]
        public Guid CartId { get; set; }

        [Display(Name = "Товар")]
        [Required(ErrorMessage ="Выберите товар")]
        public int ProductVariantId { get; set; }

        public ProductVariant ProductVariant { get; set; }

        [Display(Name = "Количество")]
        [Required(ErrorMessage ="Укажите количество")]
        [Range(1, int.MaxValue, ErrorMessage ="Количество не может быть меньше 0")]
        public int Quantity { get; set; }

        public Cart? Cart { get; set; }

   
  
    }
}
