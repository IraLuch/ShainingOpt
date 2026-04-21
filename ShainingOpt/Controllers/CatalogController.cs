using Microsoft.AspNetCore.Mvc;
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
    int? maxPrice)
        {
            var products = await _catalogService.GetProducts(); //условия
            
            if( categories != null && categories.Any())
                products = products.Where(p => categories.Contains(p.CategoryId)).ToList();

            if (brands != null && brands.Any())
                products = products.Where(p => brands.Contains(p.BrandId)).ToList();

            if (colors != null && colors.Any())
                products = products.Where(p => colors.Contains(p.ColorId)).ToList();

            if (sizes != null && sizes.Any())
                products = products.Where(p => sizes.Contains(p.SizeId)).ToList();

            if (minPrice != null)
                products = products.Where(p => p.WholesalePrice >= minPrice).ToList();

            if (maxPrice != null)
                products = products.Where(p => p.WholesalePrice <= maxPrice).ToList();

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
                MaxPrice = maxPrice
            };
            return View(model);
        }
    }
}
