using Microsoft.AspNetCore.Mvc;
using ShainingOpt.Helpers;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.Services.Interfaces;
using ShainingOpt.ViewModels;
using ShainingOpt.ViewModels.Catalog;
using System.Drawing;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ShainingOpt.Controllers
{
    /// <summary>
    /// Контроллер каталога товаров. 
    /// Отвечает за фильтрацию, поиск, пагинацию и просмотр карточки конкретного товара.
    /// </summary>
    public class CatalogController: Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
             _catalogService = catalogService;   
        }


     


        /// <summary>
        /// Вспомогательный метод для сборки комплексной ViewModel каталога.
        /// Выполняет пагинацию коллекции товаров и подгружает списки для фильтров SideBar.
        /// </summary>
        private async Task<CatalogViewModel> BuildCatalogModelAsync(List<Product> products,
            List<int>? categories = null,
    List<int>? brands = null,
    List<int>? colors = null,
    List<int>? sizes = null,
    int? minPrice = null,
    int? maxPrice = null,
    int pageSize = 15,
    int pageNumber = 1)
        {

            (int totalPages, int start, int end) = PaginationHelpers.CalculatePagination(products.Count, pageSize, pageNumber);
            products = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new CatalogViewModel{
                Products = products,
                    Colors = await _catalogService.GetColors(),
                    Brands = await _catalogService.GetBrands(),
                    Categories = await _catalogService.GetCaterories(),
                    Sizes = await _catalogService.GetSizes(),
                SelectedCategories = categories ?? new List<int>(),
                SelectedBrands = brands??  new List<int>(),
                SelectedColors = colors ?? new List<int>(),
                SelectedSizes = sizes ?? new List<int>(),

                MinPrice = minPrice,
                MaxPrice = maxPrice,

                TotalPage = totalPages,
                CurrentPage = pageNumber,
                PageStart = start,
                PageEnd = end
            };

        }


        /// <summary>
        /// Отображение основной страницы каталога с применением множественных фильтров.
        /// </summary>
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

            var model = await BuildCatalogModelAsync(products, categories, brands, colors, sizes, minPrice, maxPrice, pageSize, pageNumber);
            return View(model);
        }


        /// <summary>
        /// Карточка конкретного товара с возможностью динамического переключения торговых модификаций (вариантов).
        /// </summary>
     
        public async Task<IActionResult> Product(int productId, int? variantId)
        {
            var product = await _catalogService.GetProductWithVariants(productId);
            if (product == null)
            {
                return NotFound();
            }

            var variant = variantId == null ?
                await _catalogService.GetDefaultVariant(productId) :
                await _catalogService.GetProductVariant(variantId);

            if (variant == null)
            {
                return NotFound();
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


        /// <summary>
        /// Полнотекстовый или частичный поиск по наименованию / описанию товаров.
        /// </summary>
        public async Task<IActionResult> Search(string text)
        {
            var products = await _catalogService.GetProductsWithSearch(text.ToLower().Trim());
            var model = await BuildCatalogModelAsync(products);
            return View("Catalog", model);
        }
    }
}
