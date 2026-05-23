using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.Services.Interfaces;

namespace ShainingOpt.Services
{
    public class CatalogService : ICatalogService
    {

        private readonly AppDbContext _context;

        public CatalogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetBrands()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<List<Category>> GetCaterories()
        {
            return await _context.Categories.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<List<Color>> GetColors()
        {
            return await _context.Colors.ToListAsync();
        }

        public async Task<List<Size>> GetSizes()
        {
            return await _context.Sizes.ToListAsync();
        }


        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.Where(p => p.IsActive).Include(p => p.Category)
                 .Include(p => p.Brand)
                 .Include(p => p.Category)
                 .Include(p => p.ProductVariants)
                 .ToListAsync();
        }

        public async Task<Product> GetProductWithVariants(int productId)
        {
            return await _context.Products.
                Include(v => v.ProductVariants).ThenInclude(c => c.Color)
                .Include(v => v.ProductVariants).ThenInclude(s => s.Size).
                Include(b => b.Brand).FirstOrDefaultAsync(p => productId == p.ProductId);
        }

        public async Task<ProductVariant> GetDefaultVariant(int productId)
        {
            return await _context.ProductVariants.Include(c => c.Color).Include(s => s.Size).FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<ProductVariant> GetProductVariant(int? variantId)
        {
            return await _context.ProductVariants.Include(c => c.Color).Include(s => s.Size).FirstOrDefaultAsync(v => variantId == v.ProductVariantId);
        }

        public async Task<List<Product>> GetProductsWithSearch(string text)
        {
            var products = await GetProducts();

            return products.Where(p => p.ProductName.ToLower().Contains(text) ||
                p.Brand.BrandName.ToLower().Contains(text))
                .ToList();
        }
    }
}