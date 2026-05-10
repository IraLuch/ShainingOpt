using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;

namespace ShainingOpt.Services
{
    public class CatalogService
    {

        private readonly AppDbContext _context;

        public CatalogService(AppDbContext context)
        {
            _context = context;
        }

        internal async Task<List<Brand>> GetBrands()
        {
            return await _context.Brands.ToListAsync();
        }

        internal async Task<List<Category>> GetCaterories()
        {
            return await _context.Categories.Where(c => c.IsActive).ToListAsync();
        }

        internal async Task<List<Color>> GetColors()
        {
            return await _context.Colors.ToListAsync();
        }

        internal async Task<List<Size>> GetSizes()
        {
            return await _context.Sizes.ToListAsync();
        }


        internal async Task<List<Product>> GetProducts()
        {
           return await _context.Products.Where(p => p.IsActive).Include(p => p.Category)
                .Include(p => p.Brand)
                .Include (p => p.Category)
                .Include(p => p.ProductVariants)
                .ToListAsync();
        }

        internal async Task<Product> GetProductWithVariants(int productId)
        {
            return await _context.Products.
                Include(v => v.ProductVariants).ThenInclude(c => c.Color)
                .Include(v => v.ProductVariants).ThenInclude(s => s.Size).
                Include(b => b.Brand).FirstOrDefaultAsync(p => productId == p.ProductId);
        }

        internal async Task<ProductVariant> GetDefaultVariant(int productId)
        {
            return await _context.ProductVariants.Include(c => c.Color).Include(s => s.Size).FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        internal async Task<ProductVariant> GetProductVariant(int? variantId)
        {
            return await _context.ProductVariants.Include(c => c.Color).Include(s => s.Size).FirstOrDefaultAsync(v => variantId == v.ProductVariantId);
        }

        internal async Task<List<Product>> GetProductsWithSearch(string text)
        {
            var products = await GetProducts();

            return products.Where(p => p.ProductName.ToLower().Contains(text) ||
                p.Brand.BrandName.ToLower().Contains(text))
                .ToList();
        }
    }
}