using ShainingOpt.Models;

namespace ShainingOpt.Services.Interfaces
{
    public interface ICartService
    {
        Task AddItemToCartAsync(string cartIdString, int? userId, int variantId, int quantity);
        Task CartToUser(string cartId, int? userId);
        Task ClearCart(int userId);
        Task<Order> CreateOrderWithItems(Order order, List<OrderItem> items);
        Task DeleteVariantFromCart(string cartId, int variantId);
        Task<Cart?> GetCart(string cartId, int? userId = null);
        Task<Cart> GetCartByUser(int id);
        Task<CartItem> GetCartItemById(int cartItemId);
        Task<List<CartItem>> GetCartItems(string cartId);
        Task<Order> GetOrderById(int orderId);
        Task<List<Order>> GetOrders(int id);
        Task MergeCarts(string anonymousCartId, int userId);
        Task UpdateCartItem(CartItem cartItem, int quantity);
        Task UpdateOrderStatus(Order order, OrderStatus status);
        Task UpdateProductVariantQuantity(int orderId);
    }
}