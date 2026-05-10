using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [MinLength(6, ErrorMessage = "Длина пароля не меньше 6 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$",
    ErrorMessage = "Пароль должен содержать: хотя бы одну цифру, одну заглавную букву, одну строчную букву и один спецсимвол")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Введите номер телефона")]
        [Display(Name = "Номер телефона")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [RegularExpression(@"^(\+7|8)[0-9]{10}$", ErrorMessage = "Формат номера: +79991234567 или 89991234567")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Выберите роль")]
        [Display(Name = "Роль")]
        public int RoleId { get; set; }
    }
}
