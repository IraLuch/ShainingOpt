using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ShainingOpt.Controllers;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.Services.Interfaces;
using ShainingOpt.ViewModels.Cart;
using ShainingOpt.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Yandex.Checkout.V3;

namespace ShainingOpt.UnitTests.Controller
{
    public class CartControllerTests 
    {
        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalogService;
        private readonly IPaymentService _paymentService;
        private readonly CartController _cartController;    
        public CartControllerTests()
        {
            _cartService = A.Fake<ICartService>();
            _accountService = A.Fake<IAccountService>();
            _catalogService = A.Fake<ICatalogService>();
            _paymentService = A.Fake<IPaymentService>();

            _cartController = new CartController(_cartService, _accountService, _catalogService, _paymentService);
        }

        [Fact]
        public void CartController_Cart_ReturnView()
        {
          
            var cart = new Cart();
            A.CallTo(() => _cartService.GetCart(A<string>._)).Returns(cart);

            var result = _cartController.Cart();

            result.Should().BeOfType<Task<IActionResult>>();




        }

       

        [Fact]
        public async Task CartController_AddToCart_ReturnSuccess()
        {
            var variantId = new Random().Next();
            var productId = new Random().Next();
            var quantity = new Random().Next();

            var user = new User { Id = 10 };

            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns(Task.FromResult(user));

            A.CallTo(() => _cartService.AddItemToCartAsync(
        A<string>._,
        user.Id,
        variantId,
        quantity))
    .Returns(Task.CompletedTask);

            var result = await _cartController.AddToCart(variantId, productId, quantity);

            result.Should().BeOfType<JsonResult>();
            var json = result as JsonResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new { success = true });
        }

        [Fact]
        public async Task CartController_AddToCart_ReturnNotSuccess()
        {
            var variantId = new Random().Next();
            var productId = new Random().Next();
            var quantity = new Random().Next();

            var user = new User { Id = 10 };

            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns(Task.FromResult(user));

            A.CallTo(() => _cartService.AddItemToCartAsync(
        A<string>._,
        user.Id,
        variantId,
        quantity))
     .Throws(new Exception("DB error"));

            var result = await _cartController.AddToCart(variantId, productId, quantity);

            result.Should().BeOfType<JsonResult>();
            var json = result as JsonResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new { success = false, message = "Ошибка при добавлении в корзину" });
        }

        [Fact]
        public async Task CartController_UpdateCartItem_ReturnNotSuccess()
        {
            var updateCartItemDto = new UpdateCartItemDto();
            var cartItem = new CartItem();

            A.CallTo(() => _cartService.GetCartItemById(updateCartItemDto.CartItemId)).Returns(cartItem);
            A.CallTo(() => _cartService.UpdateCartItem(cartItem, cartItem.Quantity + updateCartItemDto.Quantity)).Throws(new Exception("DB error"));

            var result = await _cartController.UpdateCartItem(updateCartItemDto);

            result.Should().BeOfType<JsonResult>();
            var json = result as JsonResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new { success = false, message = "Ошибка при обновлении корзины" });

        }

        [Fact]
        public async Task CartController_UpdateCartItem_ReturnSuccess()
        {
            var updateCartItemDto = new UpdateCartItemDto()
            {
                CartItemId = 1,
                Quantity = 5
            };
            var cartItem = new CartItem()
            {
                CartItemId = 1,
                CartId = new Guid(),
                ProductVariantId = 3,
                Quantity = 5
            };

            A.CallTo(() => _cartService.GetCartItemById(updateCartItemDto.CartItemId)).Returns(cartItem);
            A.CallTo(() => _cartService.UpdateCartItem(cartItem, cartItem.Quantity + updateCartItemDto.Quantity)).Returns(Task.CompletedTask);

            var result = await _cartController.UpdateCartItem(updateCartItemDto);

            result.Should().BeOfType<JsonResult>();
            var json = result as JsonResult;

            json.Should().NotBeNull();
            json.Value.Should().BeEquivalentTo(new { success = true });

        }

        [Fact]
        public async Task CartController_CreateOrder_WhenModelStateInvalid_ReturnCartView()
        {
            var model = new OrderViewModel();

            var cartItem = new CartItem() { CartId = Guid.NewGuid(), ProductVariantId = 3, Quantity = 3 };

            A.CallTo(() => _cartService.GetCartItems(A<string>._)).Returns(new List<CartItem>() { cartItem });


            _cartController.ModelState.AddModelError("DeliveryAddress", "Обязательное поле");
            var result = await _cartController.CreateOrder(model);

            result.Should().BeOfType<ViewResult>();
            
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("Cart");
            viewResult.Model.Should().BeOfType<CartViewModel>();

        }

        [Fact]
        public async Task CartController_CreateOrder_WhenUserNotAuthorized_ReturnHomeView()
        {
            var model = new OrderViewModel();
 

            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns((User?)null);

            var result = await _cartController.CreateOrder(model);

            result.Should().BeOfType<RedirectToActionResult>();

            var viewResult = result as RedirectToActionResult;
            viewResult.Should().NotBeNull();
            viewResult.ActionName.Should().Be("Index");
            viewResult.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task CartController_CreateOrder_WhenOrderCreationFailed_ReturnCartView()
        {
            var model = new OrderViewModel();
            var user = new User { Id = 10 };
            var cart = new Cart();
            A.CallTo(() => _cartService.GetCart(A<string>._)).Returns(cart);

            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns(user);

            A.CallTo(() => _cartService.CreateOrderWithItems(A<Order>._, A<List<OrderItem>>._)).Returns((Order?)null);

            var result = await _cartController.CreateOrder(model);

            result.Should().BeOfType<RedirectToActionResult>();

            var viewResult = result as RedirectToActionResult;
            viewResult.Should().NotBeNull();
            viewResult.ActionName.Should().Be("Cart");


        }

        [Fact]
        public async Task CartController_CreateOrder_ReturnConfirmationUrl()
        {
            var model = new OrderViewModel();
            var user = new User { Id = 10 };

            var cart = new Cart();
            A.CallTo(() => _cartService.GetCart(
                A<string>._,
                user.Id))
            .Returns(cart);

            A.CallTo(() => _accountService.GetCurrentUserAsync(A<ClaimsPrincipal>._))
                .Returns(user);

            var order = new Order() { DeliveryAddress = "Иркутск", OrderStatus = OrderStatus.Created, TotalAmount = 1200, OrderNumber = "123"};
            A.CallTo(() => _cartService.CreateOrderWithItems(A<Order>._, A<List<OrderItem>>._)).Returns(order);

            A.CallTo(() => _cartService.CartToUser(A<string>._, A<int>._)).Returns(Task.CompletedTask);

            var payment = new Payment
            {
                Confirmation = new Confirmation
                {
                    ConfirmationUrl = "https://test.com"
                }
            };
            A.CallTo(() => _paymentService.CreatePayment(A<decimal>._, A<string>._, A<string>._, A<int>._)).Returns(payment);

            var result = await _cartController.CreateOrder(model);

            result.Should().BeOfType<RedirectResult>();

            var viewResult = result as RedirectResult;
            viewResult.Should().NotBeNull();
            viewResult.Url.Should().Be("https://test.com");
        }

    
    }
    
   


    
}
