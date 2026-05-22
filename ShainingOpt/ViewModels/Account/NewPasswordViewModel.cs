using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels.Account
{
    public class NewPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }


        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(6, ErrorMessage = "Длина пароля не меньше 6 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$",
  ErrorMessage = "Пароль должен содержать: хотя бы одну цифру, одну заглавную букву, одну строчную букву и один спецсимвол")]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
