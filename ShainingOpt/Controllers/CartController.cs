using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Prng;
using ShainingOpt.Migrations;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.Services.Configurations;
using ShainingOpt.ViewModels.Cart;
using ShainingOpt.ViewModels.Orders;

namespace ShainingOpt.Controllers
{
    /// <summary>
    /// Контроллер для работы с корзиной покупок, оформления заказов 
    /// и обработки платежей через внешние шлюзы (YooKassa).
    /// </summary>
    public class CartController : Controller
    {
        private readonly AccountService _accountService;
        private readonly CartService _cartService;
        private readonly CatalogService _catalogService;
        private readonly PaymentService _paymentService;

        public CartController(CartService cartService, AccountService accountService, 
            CatalogService catalogService, PaymentService paymentService)
        {
            _cartService = cartService;
            _accountService = accountService;
            _catalogService = catalogService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Отображение страницы корзины для текущей сессии (гостевой или авторизованной).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
      
            var cartId = GetOrCreateCartId();
            var user = await _accountService.GetCurrentUserAsync(User);
            var cart = await _cartService.GetCart(cartId, user?.Id);
            if (cart == null || cart.Items == null)
            {
                return View(new CartViewModel
                {
                    CartItems = new List<CartItem>() 
                });
            }
            var model = new CartViewModel
            {
                CartItems = cart.Items 
            };

            return View(model);
        }


        /// <summary>
        /// Добавление товара в корзину (вызывается через AJAX).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToCart(int variantId, int productId, int quantity)
        {
            var cartId = GetOrCreateCartId();
            var user = await _accountService.GetCurrentUserAsync(User);

            try
            {
                await _cartService.AddItemToCartAsync(cartId, user?.Id, variantId, quantity);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка при добавлении в корзину" });
            }
        }


        /// <summary>
        /// Получение ID корзины из Cookies или генерация нового для анонимного пользователя.
        /// </summary>
        private string GetOrCreateCartId()
        {
            var cartId = Request.Cookies["cartId"];
            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                Response.Cookies.Append("cartId", cartId);
            }
          
            return cartId;
        }

        /// <summary>
        /// Удаление определенной модификации товара из корзины.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteVariantFromCart(int variantId)
        {
            var cartId = GetOrCreateCartId();

            await _cartService.DeleteVariantFromCart(cartId, variantId);
            var cartItems = await _cartService.GetCartItems(cartId);

            var model = new CartViewModel
            {
                CartItems = cartItems
            };

            return View("Cart", model);
        }


        /// <summary>
        /// Обновление количества товара в корзине.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto model)
        {
            var cartItem = await _cartService.GetCartItemById(model.CartItemId);

            await _cartService.UpdateCartItem(cartItem, cartItem.Quantity + model.Quantity);
            return Json(new { success = true });
        }


        /// <summary>
        /// Создание заказа на основе текущей корзины и перенаправление на платежный шлюз.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderViewModel model)
        {
            var cartId = GetOrCreateCartId();
            if (!ModelState.IsValid)
            {
                var cartModel = new CartViewModel();
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                foreach (var er in errors)
                {
                    ModelState.AddModelError("CartError", er);
                }
     
                return View("Cart", new CartViewModel { CartItems = await _cartService.GetCartItems(cartId) });
            }

            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home", new { ReturnUrl = "/Cart" });
            }

            var cart = await _cartService.GetCart(cartId, user?.Id);
            var items = cart.Items;

            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 5)}",
                UserId = user.Id,
                OrderStatus = OrderStatus.Created,
                TotalAmount = items.Sum(i => i.Quantity * i.ProductVariant.Product.WholesalePrice),
                DeliveryAddress = model.DeliveryAddress,
            };

            var orderItems = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var variant = item.ProductVariant;
                variant.Quantity -= item.Quantity;

                orderItems.Add(new OrderItem
                {
                    VariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * item.ProductVariant.Product.WholesalePrice
                });
            }


            var res = await _cartService.CreateOrderWithItems(order, orderItems);
            if (res == null)
            {
                return RedirectToAction("Cart");
            }

            var returnUrl = "http://localhost:5212/Home/Index";
            await _cartService.CartToUser(cartId, order.UserId);
            var payment = _paymentService.CreatePayment(res.TotalAmount, returnUrl,
                $"Заказ {res.OrderNumber}", res.OrderId
                );

            return Redirect(payment.Confirmation.ConfirmationUrl);

        }


        /// <summary>
        /// API-эндпоинт (Webhook) для обработки уведомлений от ЮKassa.
        /// </summary>
        /// <remarks>
        /// КРИТИЧЕСКИЙ НЮАНС ДЛЯ:
        /// Для работы этого эндпоинта в локальной среде (localhost) приложение должно быть доступно из внешней сети Интернет, 
        /// чтобы серверы ЮKassa могли отправить POST-запрос. 
        /// В данном проекте проброс портов и туннелирование реализованы с помощью утилиты Cloudflare Tunnel (cloudflared daemon).
        /// URL-адрес вебхука в личном кабинете ЮKassa настраивается на выданный поддомен *.trycloudflare.com.
        /// </remarks>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("api/payment/webhook")]
        public async Task<IActionResult> Webhook()
        {
            var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            var notification = JsonConvert.DeserializeObject<YooKassaNotification>(json);


            if (notification.Event == "payment.succeeded")
            {
                var orderId = int.Parse(notification.Object.Metadata["OrderId"]);
                var order = await _cartService.GetOrderById(orderId);

                await _cartService.UpdateOrderStatus(order, OrderStatus.Processing);

                    await _cartService.UpdateProductVariantQuantity(orderId);
                    await _cartService.ClearCart(order.UserId);
                
            }

            return Ok();

        }

        /// <summary>
        /// Страница со списком заказов пользователя.
        /// </summary>
        public async Task<IActionResult> Order()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null)
            {
                return View("Index", "Home");
            }
            var orders = await _cartService.GetOrders(user.Id);
            var model = new OrderViewModel
            {
              Orders = orders
            };
            return View(model);
        }

       
    }
}
