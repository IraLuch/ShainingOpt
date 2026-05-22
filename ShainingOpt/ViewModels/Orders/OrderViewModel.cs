using ShainingOpt.Models;
using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels.Orders
{
    public class OrderViewModel
    {
        
        [Display(Name = "Адрес доставки")]
        [Required(ErrorMessage = "Введите адрес доставки")]
        public string DeliveryAddress { get; set; }

        public List<Order> Orders { get; set; } = new();

        //public Order Order { get; set; }

        //public User User { get; set; }


    }
}
