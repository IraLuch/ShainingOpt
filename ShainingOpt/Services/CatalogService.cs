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

        internal async Task<List<Product>> GetAllProducts()
        {
           return await _context.Products.Where(p => p.IsActive).ToListAsync();
        }
    }
}