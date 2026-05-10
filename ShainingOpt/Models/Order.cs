using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShainingOpt.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Display(Name = "Номер заказа")]
        public string? OrderNumber { get; set; }

        [Display(Name = "Пользователь")]
        public int UserId { get; set; }

        [Display(Name = "Статус")]
        [Required(ErrorMessage = "Выберите статус")]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Created;

        [Display(Name = "Сумма")]
        [Required(ErrorMessage = "Введите сумму заказа")]
        [Range(0.01, 999999, ErrorMessage = "Сумма должна быть больше 0")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Адрес доставки")]
        [Required(ErrorMessage = "Введите адрес доставки")]
        public string DeliveryAddress { get; set; }

        [Display(Name = "Дата заказа")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;


        [Display(Name = "Пользователь")]
        [Required(ErrorMessage = "Пользователь не указан")]
        public User? User { get; set; }

        [Display(Name = "Состав заказа")]
        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public enum OrderStatus
    {
        [Display(Name = "Создан")]
        Created = 0,

        [Display(Name = "В обработке")]
        Processing = 1,

        [Display(Name = "Отправлен")]
        Shipped = 2,

        [Display(Name = "Доставлен")]
        Delivered = 3,

        [Display(Name = "Выдан")]
        Completed = 4,

        [Display(Name = "Отменен")]
        Cancelled = 5
    }
}
