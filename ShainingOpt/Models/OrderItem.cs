using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Display(Name = "Заказ")]
        [Required(ErrorMessage ="Введите номер заказа")]
        public int OrderId { get; set; }

        [Display(Name = "Товар")]
        [Required(ErrorMessage ="Выберите товар")]
        public int VariantId { get; set; }

        [Display(Name = "Количество")]
        [Required(ErrorMessage ="Укажите количество")]
        [Range(1, int.MaxValue, ErrorMessage ="Количсетво не может быть меньше либо равно 0")]
        public int Quantity { get; set; }

        [Display(Name = "Сумма")]
        [Required(ErrorMessage ="Укажите сумму")]
        [Range(0.01, 999999, ErrorMessage = "Сумма должна быть больше 0")]
        public decimal TotalPrice { get; set; }

        public Order? Order { get; set; }
        public ProductVariant? Variant { get; set; }
    }
}
