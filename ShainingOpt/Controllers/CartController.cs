using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShainingOpt.Migrations;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.ViewModels;

namespace ShainingOpt.Controllers
{
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
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
           
            var cartId = Request.Cookies["cartId"];
            if (cartId == null)
            {
                return View(new CartViewModel());
            }
            var cart = await _cartService.GetCartById(cartId);

            var cartItems = await _cartService.GetCartItems(cartId);

            var model = new CartViewModel
            {
                CartItems = cartItems
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int variantId, int productId, int quantity)
        {
            string cartId;
            if (!Request.Cookies.ContainsKey("cartId"))
            {
                cartId = Guid.NewGuid().ToString();
                Response.Cookies.Append("cartId", cartId);
                await _cartService.CreateCart(cartId);
            }
            else
            {
                cartId = Request.Cookies["cartId"];
            }

            var cart = await _cartService.GetCartById(cartId);
            var product = await _catalogService.GetProductWithVariants(productId);


            var cartItems = await _cartService.GetCartItems(cartId);

            var cartItem = cartItems.FirstOrDefault(i => i.ProductVariantId == variantId);

            if (cartItem != null)
            {
                await _cartService.UpdateCartItem(cartItem, quantity);
            }
            else
            {
                 cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductVariantId = variantId,
                    Quantity = quantity,

                };
                await _cartService.AddItemToCart(cartItem);

            }


            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVsriantFromCart(int variantId)
        {
            var cartId = Request.Cookies["cartId"];

            await _cartService.DeleteVariantFromCart(cartId, variantId);
            var cartItems = await _cartService.GetCartItems(cartId);

            var model = new CartViewModel
            {
                CartItems = cartItems
            };

            return View("Cart", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto model)
        {
            var cartItem = await _cartService.GetCartItemById(model.CartItemId);

            await _cartService.UpdateCartItem(cartItem, model.Quantity);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderViewModel model)
        {
            var cartId = Request.Cookies["cartId"];
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

            var cart = await _cartService.GetCartById(cartId);
            var items = await _cartService.GetCartItems(cartId);

            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 5)}",
                CompanyId = user.Company.CompanyId,
                UserId = user.Id,
                OrderStatus = OrderStatus.Created,
                TotalAmount = items.Sum(i => i.Quantity * i.ProductVariant.Product.WholesalePrice),
                DeliveryAddress = model.DeliveryAddress,
            };

            var orderItems = items.Select(i => new OrderItem { 
               
                VariantId = i.ProductVariantId,
                Quantity = i.Quantity,
                TotalPrice = i.Quantity * i.ProductVariant.Product.WholesalePrice

            }).ToList();



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
                if (order == null)
                {
                    
                    return Ok();
                }

                await _cartService.UpdateOrderStatus(order, OrderStatus.Processing);

                    await _cartService.UpdateProductVariantQuantity(orderId);
                    await _cartService.ClearCart(order.UserId);
                
            }

            return Ok();

        }

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
