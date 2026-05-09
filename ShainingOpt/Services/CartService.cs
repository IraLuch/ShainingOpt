using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using ShainingOpt.DataBase;
using ShainingOpt.Models;

namespace ShainingOpt.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;
        public CartService(AppDbContext context)
        {
            _context = context;
        }

        internal async Task AddItemToCart(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        internal async Task CreateCart(string cartId, int? userId)
        {
            var cart = _context.Carts.Add(new Cart { CartId = Guid.Parse(cartId) , UserId = userId});
            await _context.SaveChangesAsync();
        }

        internal async Task DeleteVariantFromCart(string cartId, int variantId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == Guid.Parse(cartId) && c.ProductVariantId == variantId);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        internal async Task<Cart?> GetCart(string cartId, int? userId = null)
        {
            var query = _context.Carts
        .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
                .ThenInclude(pv => pv.Product)
        .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
                .ThenInclude(pv => pv.Size)
        .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
                .ThenInclude(pv => pv.Color)
        .AsQueryable();
            if ( userId != null)
            {
            return await query.FirstOrDefaultAsync(c => c.UserId == userId);
            }
           if (cartId != null)
            {

            return await query.FirstOrDefaultAsync(c => c.CartId == Guid.Parse(cartId));
            }
            return null;

        }

        internal async Task<List<CartItem>> GetCartItems(string cartId)
        {
            return await _context.CartItems.Include(v => v.ProductVariant).ThenInclude(pv => pv.Product)
                .Include(v => v.ProductVariant).ThenInclude(pv => pv.Size)
                 .Include(v => v.ProductVariant).ThenInclude(pv => pv.Color)
                .Where(c => c.CartId == Guid.Parse(cartId)).ToListAsync();   
        }

        public async Task<CartItem> GetCartItemById(int cartItemId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(i => i.CartItemId == cartItemId);
        }

        internal async Task UpdateCartItem(CartItem cartItem, int quantity)
        {
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
        }

        internal async Task<Order> CreateOrderWithItems(Order order, List<OrderItem> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
                foreach (var item in items)
                {
                    item.OrderId = order.OrderId;
                }

                _context.OrderItems.AddRange(items);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;

            }
            catch(Exception ex) 
            {
              await transaction.RollbackAsync();
                return null;
            }
        }

        internal async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        internal async Task UpdateOrderStatus(Order order, OrderStatus status)
        {
            order.OrderStatus = status;
            await _context.SaveChangesAsync();
        }

        internal async Task ClearCart(int userId)
        {
            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }

        }

        internal async Task UpdateProductVariantQuantity(int orderId)
        {
            var orderItems = await _context.OrderItems
        .Where(o => o.OrderId == orderId)
        .ToListAsync();

            foreach (var item in orderItems)
            {
                var variant = await _context.ProductVariants.FirstOrDefaultAsync(p => p.ProductVariantId == item.VariantId);
                if (variant != null)
                {
                    variant.Quantity -= item.Quantity;
                }

            }
            await _context.SaveChangesAsync();
        }

        internal async Task CartToUser(string cartId, int? userId)
        {
            var cart = await GetCart(cartId);
            cart.UserId = userId;
            await _context.SaveChangesAsync();
        }

        internal async Task<List<OrderItem>> GetOrderItems(int id)
        {
            return await _context.OrderItems.Include(o => o.Order).Include(o => o.Variant).ThenInclude(pv => pv.Product)
                .Include(o => o.Variant).ThenInclude(pv => pv.Size)
                 .Include(v => v.Variant).ThenInclude(pv => pv.Color).Where(o => o.Order.UserId == id).ToListAsync();
        }

        internal async Task<List<Order>> GetOrders(int id)
        {
            return await _context.Orders.Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
                .ThenInclude(v => v.Product)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Variant)
                .ThenInclude(v => v.Size)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Variant)
                .ThenInclude(v => v.Color)
                .Where(o => o.UserId == id).ToListAsync();
        }

        internal async Task<Cart> GetCartByUser(int id)
        {
            return await _context.Carts.Include(o => o.User).Include(o => o.Items).FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task AddItemToCartAsync(string cartIdString, int? userId, int variantId, int quantity)
        {
            if (!Guid.TryParse(cartIdString, out Guid cartId)) return;

            // 1. Получаем корзину со всеми связями
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            // 2. Если корзины нет — создаем
            if (cart == null)
            {
                cart = new Cart { CartId = cartId, UserId = userId };
                _context.Carts.Add(cart);
            }
            else if (cart.UserId == null && userId != null)
            {
                cart.UserId = userId; // Привязываем анонимную корзину к юзеру
            }

            // 3. Работаем с товаром
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == variantId);

            if (cartItem != null)
            {
                // Логика ограничения по остаткам
                int totalRequested = cartItem.Quantity + quantity;
                cartItem.Quantity = Math.Min(totalRequested, cartItem.ProductVariant.Quantity);
            }
            else
            {
                // Добавляем новый
                cart.Items.Add(new CartItem
                {
                    ProductVariantId = variantId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}