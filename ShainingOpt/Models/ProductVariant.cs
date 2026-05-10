using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }

        [Required(ErrorMessage = "Введите продукт" )]
        [Display(Name ="Товар")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int ColorId { get; set; }

        [Display(Name = "Цвет")]
        [Required(ErrorMessage = "Выберите цвет")]
        public Color? Color { get; set; }

        [Display(Name = "Размер")]
        [Required(ErrorMessage = "Выберите размер")]
        public int SizeId { get; set; }
        public Size? Size { get; set; }

        [Display(Name = "Количество на складе")]
        [Required(ErrorMessage = "Укажите количество")]
        [Range(1, int.MaxValue, ErrorMessage = "Количсетво не может быть меньше либо равно 0")]
        public int Quantity { get; set; }

        [Display(Name = "Минимальное количество на складе")]
        [Required(ErrorMessage = "Укажите количество")]
        [Range(1, int.MaxValue, ErrorMessage = "Количсетво не может быть меньше либо равно 0")]
        public int MinOrderQuantity { get; set; } = 20;

        [Display(Name = "Фото варианта")]
        public string? ImageUrl { get; set; }  

    }
}
