using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ShainingOpt.Controllers;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
        public void CartController_Cart_ReturnSuccess()
        {
            var id = new Guid().ToString();
            var cart = A.Fake<Cart>();
            A.CallTo(() => _cartService.GetCart(id)).Returns(cart);

            var result = _cartController.Cart();

            result.Should().BeOfType<Task<IActionResult>>();


        }

    }
    
   


    
}
