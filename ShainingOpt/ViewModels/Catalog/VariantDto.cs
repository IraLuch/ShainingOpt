namespace ShainingOpt.ViewModels.Catalog
{
    public class VariantDto
    {
        public int ProductVariantId { get; set; }

        public int ColorId { get; set; }
        public string ColorName { get; set; }

        public int SizeId { get; set; }
        public string SizeName { get; set; }

        public string? ImageUrl { get; set; }

        public int Quantity { get; set; }
        public int MinOrderQuantity { get; set; }
    }
}
