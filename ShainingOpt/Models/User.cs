using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ShainingOpt.Models
{
    public class User: IdentityUser<int>
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public override string Email { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        public override string PhoneNumber { get; set; }

        [Display(Name = "Роль")]
        public Role? Role { get; set; }

        public int? RoleId { get; set; }

        public Company? Company { get; set; }
        public Cart? Cart { get; set; }
    }
}

