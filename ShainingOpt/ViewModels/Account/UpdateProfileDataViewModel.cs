using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels.Account
{
    public class UpdateProfileDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Название компании")]
        [Required(ErrorMessage = "Введите название компании")]
        public string CompanyName { get; set; }

        [Display(Name = "ИНН")]
        [Required(ErrorMessage = "Введите ИНН")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "ИНН должен состоять только из 10 цифр")]
        public string Inn { get; set; }

        [Display(Name = "КПП")]
        [Required(ErrorMessage = "Введите КПП")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "КПП должен состоять только из 9 цифр")]
        public string Kpp { get; set; }

        [Display(Name = "Юридический адрес")]
        [Required(ErrorMessage = "Введите юридический адрес")]
        public string Address { get; set; }

        [Display(Name = "Контактное лицо")]
        [Required(ErrorMessage = "Введите контактное лицо")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [RegularExpression(@"^(\+7|8)[0-9]{10}$", ErrorMessage = "Формат номера: +79991234567 или 89991234567")]
        public string Phone { get; set; }
    }
}
