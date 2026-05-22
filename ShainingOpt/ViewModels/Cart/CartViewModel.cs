using ShainingOpt.Models;

namespace ShainingOpt.ViewModels.Cart
{
    public class CartViewModel
    {
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public decimal TotalSum => CartItems.Sum(x => x.Quantity * x.ProductVariant.Product.WholesalePrice);

        public int CartItemsCount => CartItems.Count;

        

    }
}
