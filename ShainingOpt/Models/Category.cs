using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "Название категории")]
        [Required(ErrorMessage = "Введите название категории")]
        public string CategoryName { get; set; }

        [Display(Name = "Активна")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Товары")]
        public ICollection<Product>? Products { get; set; } = new List<Product>();
    }
}
