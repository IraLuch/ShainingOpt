
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.Services.Interfaces;

namespace ShainingOpt.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteVariantFromCart(string cartId, int variantId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == Guid.Parse(cartId) && c.ProductVariantId == variantId);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task MergeCarts(string anonymousCartId, int userId)
        {
            if (!Guid.TryParse(anonymousCartId, out Guid guidCartId)) return;

            var anonymousCart = await _context.Carts
                .Include(c => c.Items).ThenInclude(c => c.ProductVariant)
                .FirstOrDefaultAsync(c => c.CartId == guidCartId && c.UserId == null);

            var userCart = await _context.Carts
                .Include(c => c.Items).ThenInclude(c => c.ProductVariant)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (anonymousCart != null && anonymousCart.Items.Any())
            {

                if (userCart == null)
                {
                    anonymousCart.UserId = userId;
                }
                else
                {

                    foreach (var item in anonymousCart.Items)
                    {
                        var existingItem = userCart.Items
                            .FirstOrDefault(i => i.ProductVariantId == item.ProductVariantId);

                        if (existingItem != null)
                        {
                            var maxQuntity = item.ProductVariant.Quantity;
                            var needQuntity = existingItem.Quantity + item.Quantity;
                            existingItem.Quantity = Math.Min(maxQuntity, needQuntity);

                        }
                        else
                        {
                            item.CartId = userCart.CartId;
                            userCart.Items.Add(item);
                        }
                    }
                    _context.Carts.Remove(anonymousCart);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart?> GetCart(string cartId, int? userId = null)
        {

            var guidCartId = Guid.Parse(cartId);

            var cart = await _context.Carts.Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
                .ThenInclude(pv => pv.Product)
        .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
                .ThenInclude(pv => pv.Size)
        .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
                .ThenInclude(pv => pv.Color).FirstOrDefaultAsync(c => (userId != null && c.UserId == userId) || c.CartId == guidCartId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = guidCartId,
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            else if (userId != null && cart.UserId == null)
            {
                cart.UserId = userId;
                await _context.SaveChangesAsync();
            }

            return cart;

        }

        public async Task<List<CartItem>> GetCartItems(string cartId)
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

        public async Task UpdateCartItem(CartItem cartItem, int quantity)
        {
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
        }

        public async Task<Order> CreateOrderWithItems(Order order, List<OrderItem> items)
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
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task UpdateOrderStatus(Order order, OrderStatus status)
        {
            order.OrderStatus = status;
            await _context.SaveChangesAsync();
        }

        public async Task ClearCart(int userId)
        {
            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }

        }

        public async Task UpdateProductVariantQuantity(int orderId)
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

        public async Task CartToUser(string cartId, int? userId)
        {
            var cart = await GetCart(cartId);
            cart.UserId = userId;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrders(int id)
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

        public async Task<Cart> GetCartByUser(int id)
        {
            return await _context.Carts.Include(o => o.User).Include(o => o.Items).FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task AddItemToCartAsync(string cartIdString, int? userId, int variantId, int quantity)
        {
            if (!Guid.TryParse(cartIdString, out Guid guidCartId)) return;

            //Ищем корзину: сначала по UserId, если нет — по CartId
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => (userId != null && c.UserId == userId) || c.CartId == guidCartId);

            //Если корзины нет — создаем её
            if (cart == null)
            {
                cart = new Cart { CartId = guidCartId, UserId = userId };
                _context.Carts.Add(cart);
            }
            //Если нашли анонимную корзину, а пользователь залогинен
            else if (cart.UserId == null && userId != null)
            {
                cart.UserId = userId;
            }

            //Ищем, есть ли уже такой товар в корзине
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == variantId);

            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant == null) return;

            if (cartItem != null)
            {
                int totalRequested = cartItem.Quantity + quantity;
                cartItem.Quantity = Math.Min(totalRequested, variant.Quantity);
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductVariantId = variantId,
                    Quantity = Math.Min(quantity, variant.Quantity)
                });
            }

            await _context.SaveChangesAsync();
        }


    }
}