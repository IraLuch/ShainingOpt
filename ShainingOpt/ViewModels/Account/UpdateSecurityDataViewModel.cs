using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels.Account
{
    public class UpdateSecurityDataViewModel
    {
        [Required(ErrorMessage = "Введите новый пароль")]
        [MinLength(6, ErrorMessage = "Длина пароля не меньше 6 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$",
 ErrorMessage = "Пароль должен содержать: хотя бы одну цифру, одну заглавную букву, одну строчную букву и один спецсимвол")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Подтверждение пароля обязательно")]

        [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
    }
}
