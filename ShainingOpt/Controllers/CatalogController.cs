using Microsoft.AspNetCore.Mvc;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.ViewModels;

namespace ShainingOpt.Controllers
{
    public class CatalogController: Controller
    {
        private readonly CatalogService _catalogService;

        public CatalogController(CatalogService catalogService)
        {
             _catalogService = catalogService;   
        }

        [HttpGet]
        public async  Task<IActionResult> Catalog(
            List<int> categories ,
    List<int> brands,
    List<int> colors,
    List<int> sizes,
    int? minPrice,
    int? maxPrice,
    int pageSize = 15,
    int pageNumber = 1)

        
        {
            var products = await _catalogService.GetProducts(); 


            if ( categories != null && categories.Any())
                products = products.Where(p => categories.Contains(p.CategoryId)).ToList();

            if (brands != null && brands.Any())
                products = products.Where(p => brands.Contains(p.BrandId)).ToList();

            if (colors != null && colors.Any())
                products = products.Where(p => p.ProductVariants.Any(c => colors.Contains(c.ColorId))).ToList();

            if (sizes != null && sizes.Any())
                products = products.Where(p => p.ProductVariants.Any(c => sizes.Contains(c.SizeId))).ToList();

            if (minPrice != null)
                products = products.Where(p => p.WholesalePrice >= minPrice).ToList();

            if (maxPrice != null)
                products = products.Where(p => p.WholesalePrice <= maxPrice).ToList();

            int totalPages = (int)Math.Ceiling(products.Count / (double)pageSize);

            products = products.Skip(pageNumber - 1).Take(pageSize).ToList();

            var start = Math.Max(1, pageNumber - 2);
            var end = Math.Min(pageNumber + 2, totalPages);

            var model = new CatalogViewModel
            {
                Products = products,
                Colors = await _catalogService.GetColors(),
                Brands = await _catalogService.GetBrands(),
                Categories = await _catalogService.GetCaterories(),
                Sizes = await _catalogService.GetSizes(),

                SelectedCategories = categories,
                SelectedBrands = brands,
                SelectedColors = colors,
                SelectedSizes = sizes,

                MinPrice = minPrice,
                MaxPrice = maxPrice,

                TotalPage = totalPages,
                CurrentPage = pageNumber,
                PageStart = start,
                PageEnd = end
            };
            return View(model);
        }

        public async Task<IActionResult> Product(int productId, int? variantId)
        {
            var product = await _catalogService.GetProductWithVariants(productId);
            if (product == null)
            {
                return View("Index"); //страница ошибки
            }

            var variant = variantId == null ?
                await _catalogService.GetDefaultVariant(productId) :
                await _catalogService.GetProductVariant(variantId);

            if (variant == null)
            {
                return View("Index"); //страницв ошибки
            }

            var model = new ProductViewModel
            {
                ProductId = productId,   
                WholesalePrice = product.WholesalePrice,
                Brand = product.Brand.BrandName,
                ProductName = product.ProductName,
                Description = product.Description,
                SelectedVariant = new VariantDto
                {
                    ProductVariantId = variant.ProductVariantId,
                    ColorId = variant.ColorId,
                    ColorName = variant.Color.ColorName,
                    SizeName = variant.Size.SizeName,
                    SizeId = variant.SizeId,
                    ImageUrl = variant.ImageUrl,
                    Quantity = variant.Quantity,
                    MinOrderQuantity= variant.MinOrderQuantity,
                },
                Variants = product.ProductVariants.Select(v => new VariantDto
                {
                    ProductVariantId = v.ProductVariantId,
                    ColorId = v.ColorId,
                    SizeId = v.SizeId,
                    ColorName = v.Color.ColorName,
                    SizeName = v.Size.SizeName,
                    ImageUrl = v.ImageUrl,
                    Quantity = v.Quantity,
                    MinOrderQuantity = v.MinOrderQuantity
                }).ToList()
            };
            return View(model);
        }
    }
}
