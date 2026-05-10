using ShainingOpt.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ShainingOpt.ViewModels
{
    public class EditOrderViewModel
    {

        public Order Order { get; set; }

        [Display(Name = "Товары")]
        public List<ProductVariant> Products { get; set; }
    }
}
