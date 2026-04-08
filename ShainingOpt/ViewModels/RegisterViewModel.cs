using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Название компании")]
        [Required(ErrorMessage = "Введите название компании")]
        public string CompanyName { get; set; }

        [Display(Name = "ИНН")]
        [Required(ErrorMessage = "Введите ИНН")]
        [StringLength(10, ErrorMessage = "ИНН должен содержать 10 символов")]
        public string Inn { get; set; }

        [Display(Name = "КПП")]
        [Required(ErrorMessage = "Введите КПП")]
        [StringLength(9, ErrorMessage = "КПП должен содержать 9 символов")]
        public string Kpp { get; set; }

        [Display(Name = "Юридический адрес")]
        [Required(ErrorMessage = "Введите юридический адрес")]
        public string LegalAddress { get; set; }

        [Display(Name = "Контактное лицо")]
        [Required(ErrorMessage = "Введите контактное лицо")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage ="Введите пароль")]
        public string Password { get; set; }
    }
}
