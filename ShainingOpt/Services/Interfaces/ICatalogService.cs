using ShainingOpt.Models;

namespace ShainingOpt.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<List<Brand>> GetBrands();
        Task<List<Category>> GetCaterories();
        Task<List<Color>> GetColors();
        Task<ProductVariant> GetDefaultVariant(int productId);
        Task<List<Product>> GetProducts();
        Task<List<Product>> GetProductsWithSearch(string text);
        Task<ProductVariant> GetProductVariant(int? variantId);
        Task<Product> GetProductWithVariants(int productId);
        Task<List<Size>> GetSizes();
    }
}