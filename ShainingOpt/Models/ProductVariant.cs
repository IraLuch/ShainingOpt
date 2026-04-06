using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class ProductVariant
    {
        [Key]
        public int ProductVariantId { get; set; }

        [Display(Name = "Товар")]
        [Required(ErrorMessage = "Не указан товар")]
        public int ProductId { get; set; }

        [Display(Name = "Цвет")]
        [Required(ErrorMessage = "Выберите цвет")]
        public int ColorId { get; set; }

        [Display(Name = "Размер")]
        [Required(ErrorMessage = "Выберите размер")]
        public int SizeId { get; set; }

        [Display(Name = "Изображение")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Количество")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество должно не должно быть меньше 0")]
        public int Quantity { get; set; }

        public Product? Product { get; set; }
        public Color? Color { get; set; }
        public Size? Size { get; set; }
    }
}
