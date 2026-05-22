using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels.Account
{
    public class ProfileViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Введите название компании")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Введите контактное лицо")]
        public string? ContactPerson { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [RegularExpression(@"^(\+7|8)[0-9]{10}$", ErrorMessage = "Формат номера: +79991234567 или 89991234567")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Введите КПП")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "КПП должен состоять только из 9 цифр")]
        public string? Kpp { get; set; }

        [Required(ErrorMessage = "Введите ИНН")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "ИНН должен состоять только из 10 цифр")]
        public string? Inn { get; set; }

        [Required(ErrorMessage = "Введите юридический адрес")]
        public string? Address { get; set; }

      
        public string? Password { get; set; }

        [MinLength(6, ErrorMessage = "Длина пароля не меньше 6 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).+$",
ErrorMessage = "Пароль должен содержать: хотя бы одну цифру, одну заглавную букву, одну строчную букву и один спецсимвол")]
        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }
    }
}
