using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }

         [Required(ErrorMessage ="Введите пароль")]
        public string Password { get; set; }
    }
}