using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        public int UserId { get; set; }

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


        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;



       
        [Required(ErrorMessage = "Пользователь не указан")]
        [Display(Name = "Пользователь")]
        public User? User { get; set; }
    }
}
