using ShainingOpt.Models;
using System.ComponentModel.DataAnnotations;

namespace ShainingOpt.ViewModels.Catalog
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        // 🔹 основная инфа о товаре
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public decimal WholesalePrice { get; set; }

        // 🔹 выбранный вариант
        public VariantDto SelectedVariant { get; set; }

        // 🔹 ВСЕ варианты
        public List<VariantDto> Variants { get; set; } = new();
    }
}
