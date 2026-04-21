using Microsoft.AspNetCore.Mvc;
using ShainingOpt.Services;

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
        public async  Task<IActionResult> Catalog()
        {
            var products = await _catalogService.GetAllProducts();
            return View(products);
        }
    }
}
