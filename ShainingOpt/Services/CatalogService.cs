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
                .Include(p => p.Color)
                .Include(p => p.Size)
                .ToListAsync();
        }
    }
}