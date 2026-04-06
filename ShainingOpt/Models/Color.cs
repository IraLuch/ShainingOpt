using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Color
    {
        [Key]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Цвета обязателен")]
        [Display(Name = "Цвет")]
        public required string ColorName { get; set; }

        public ICollection<ProductVariant>? Variants { get; set; } = new List<ProductVariant>();

    }
}
