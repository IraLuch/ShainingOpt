using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Size
    {
        [Key]
        public int SizeId { get; set; }

        [Required(ErrorMessage = "Размера обязателен")]
        [Display(Name = "Размер")]
        public  string SizeName { get; set; }

    }
}
