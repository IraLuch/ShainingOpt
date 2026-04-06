using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Role: IdentityRole<int>
    {
        [Required(ErrorMessage ="Введите название роли")]
        [Display(Name= "Название роли")]
        public override string Name { get; set; }
    }
}
