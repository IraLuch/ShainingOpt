using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;

namespace ShainingOpt.Areas_Manager_Controllers
{
    [Area("Manager")]
    public class ProductVariantsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ProductVariantsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: ProductVariants
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ProductVariants.Include(p => p.Color).Include(p => p.Product).Include(p => p.Size);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ProductVariants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariant = await _context.ProductVariants
                .Include(p => p.Color)
                .Include(p => p.Product)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ProductVariantId == id);
            if (productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        // GET: ProductVariants/Create
        public IActionResult Create()
        {
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName");
            ViewBag.ImgBbKey = _configuration["ImgBB:ApiKey"];
            return View();
        }

        // POST: ProductVariants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductVariantId,ProductId,ColorId,SizeId,Quantity,MinOrderQuantity,ImageUrl")] ProductVariant productVariant)
        {
            ModelState.Remove("Color");
            if (ModelState.IsValid)
            {
                _context.Add(productVariant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName", productVariant.ColorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productVariant.ProductId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName", productVariant.SizeId);
            return View(productVariant);
        }

        // GET: ProductVariants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariant = await _context.ProductVariants.FindAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            ViewBag.ImgBbKey = _configuration["ImgBB:ApiKey"];
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName", productVariant.ColorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productVariant.ProductId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName", productVariant.SizeId);
            return View(productVariant);
        }

        // POST: ProductVariants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductVariantId,ProductId,ColorId,SizeId,Quantity,MinOrderQuantity,ImageUrl")] ProductVariant productVariant)
        {
            if (id != productVariant.ProductVariantId)
            {
                return NotFound();
            }

            ModelState.Remove("Color");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productVariant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductVariantExists(productVariant.ProductVariantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName", productVariant.ColorId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productVariant.ProductId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "SizeName", productVariant.SizeId);
            return View(productVariant);
        }

        // GET: ProductVariants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariant = await _context.ProductVariants
                .Include(p => p.Color)
                .Include(p => p.Product)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ProductVariantId == id);
            if (productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        // POST: ProductVariants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productVariant = await _context.ProductVariants.FindAsync(id);
            if (productVariant != null)
            {
                _context.ProductVariants.Remove(productVariant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductVariantExists(int id)
        {
            return _context.ProductVariants.Any(e => e.ProductVariantId == id);
        }
    }
}
