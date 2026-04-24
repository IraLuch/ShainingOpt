using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShainingOpt.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Display(Name = "Название товара")]
        [Required(ErrorMessage = "Введите название товара")]
        public string ProductName { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Выберите категорию")]
        public int CategoryId { get; set; }

        [Display(Name = "Бренд")]
        [Required(ErrorMessage = "Выберите бренд")]
        public int BrandId { get; set; }

        [Display(Name = "Оптовая цена")]
        [Required(ErrorMessage = "Введите цену")]
        [Range(0.01, 999999, ErrorMessage = "Цена должна быть больше 0" )]
        public decimal WholesalePrice { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Изображение")]
        public string? MainImageUrl { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Category? Category { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<ProductVariant>? ProductVariants { get; set; } = new List<ProductVariant>();
    }
}
