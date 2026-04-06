using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Название бренда обязательно")]
        public string BrandName { get; set; }
    }
}
