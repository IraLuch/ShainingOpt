using ShainingOpt.Models;

namespace ShainingOpt.ViewModels
{
    public class CatalogViewModel
    {
        public List<Brand> Brands { get; set; }

        public List<Product> Products { get; set; }

        public List<Color> Colors { get; set; }

        public List<Category> Categories { get; set; }

        public List<Size> Sizes { get; set; }

        public List<int> SelectedCategories { get; set; } = new();
        public List<int> SelectedBrands { get; set; } = new();
        public List<int> SelectedColors { get; set; } = new();
        public List<int> SelectedSizes { get; set; } = new();

        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }

        public int TotalPage {  get; set; }
        public int CurrentPage {  get; set; }
         public int PageStart { get; set; }
        public int PageEnd { get; set; }

    }
}
