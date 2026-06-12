using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Yandex.Checkout.V3;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShainingOpt.UnitTests.Services
{
    public class CartServiceTest
    {
        private async Task<AppDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .ConfigureWarnings(w =>
        w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
    .Options;

            var databaseContext = new AppDbContext(options);

            databaseContext.Database.EnsureCreated();

            return databaseContext;

        }

        private async Task<(AppDbContext dbContext, CartService cartService)> GetCartService()
        {
            var dbContext = await GetDbContext();
            var cartService = new CartService(dbContext);
            return (dbContext, cartService);
        }

        [Fact]
        public async Task CartService_GetCart_ReturnExistingCart()
        {
            //Arrange
            var data = await GetCartService();

            var cart = new Cart();

            data.dbContext.Carts.Add(cart);
            await data.dbContext.SaveChangesAsync();

            //Act
            var result = await data.cartService.GetCart(cart.CartId.ToString());

            //Assert
            result.Should().NotBeNull();
            result.CartId.Should().Be(cart.CartId);
            result.Items.Should().NotBeNull();

        }


        [Fact]
        public async Task CartService_GetCart_ReturnNewCart_WhenCartNotExists()
        {
            //Arrange
            var data = await GetCartService();
            var cartId = Guid.NewGuid();

            //Act
            var result = await data.cartService.GetCart(cartId.ToString());

            //Assert
            result.Should().NotBeNull();
            result.CartId.Should().Be(cartId);
            data.dbContext.Carts.Count().Should().Be(1);
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task CartService_GetCart_AttachAnonymousCartToUser()
        {
            //Arrange
            var data = await GetCartService();

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };
            data.dbContext.Users.Add(user);

            var cart = new Cart();
            data.dbContext.Carts.Add(cart);

            await data.dbContext.SaveChangesAsync();


            //Act
            var result = await data.cartService.GetCart(cart.CartId.ToString(), user.Id);

            //Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(user.Id);
            result.Items.Should().NotBeNull();
        }

        [Fact]
        public async Task CartService_AddItemToCartAsync_WhereCartNotExists()
        {
            //Arrange
            var data = await GetCartService();

            var cartId = Guid.NewGuid();

            var product = await CreateProductWithVariant(data.dbContext);

            //Act
            await data.cartService.AddItemToCartAsync(cartId.ToString(), null, product.variant.ProductVariantId, 5);

            //Assert
            data.dbContext.Carts.Count().Should().Be(1);
            data.dbContext.Carts.First().CartId.Should().Be(cartId);
            data.dbContext.Carts.First().Items.Should().NotBeNullOrEmpty();
            data.dbContext.Carts.First().Items.First().ProductVariantId.Should().Be(product.variant.ProductVariantId);
        }

        [Fact]
        public async Task CartService_AddItemToCartAsync_WhereItemExists()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

          
            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.AddItemToCartAsync(cart.cart.CartId.ToString(), null, product.variant.ProductVariantId, 5);

            //Assert
            data.dbContext.Carts.First().Items.Should().NotBeNullOrEmpty();
            data.dbContext.Carts.First().Items.Count().Should().Be(1);
            data.dbContext.Carts.First().Items.First().Quantity.Should().Be(10);


        }

        [Fact]
        public async Task CartService_AddItemToCartAsync_WhereItemMaxValue()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);
            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.AddItemToCartAsync(cart.cart.CartId.ToString(), null, product.variant.ProductVariantId, 15);

            //Assert
            data.dbContext.Carts.First().Items.Should().NotBeNullOrEmpty();
            data.dbContext.Carts.First().Items.Count().Should().Be(1);
            data.dbContext.Carts.First().Items.First().Quantity.Should().Be(10);


        }

        [Fact]
        public async Task CartService_AddItemToCartAsync_WhereItemNotExists()
        {
            //Arrange
            var data = await GetCartService();

            var cart = new Cart();

            data.dbContext.Carts.Add(cart);
            await data.dbContext.SaveChangesAsync();

            var randomVariantId = new Random().Next();

            //Act
            await data.cartService.AddItemToCartAsync(cart.CartId.ToString(), null, randomVariantId, 5);

            //Assert
            data.dbContext.Carts.First().Items.Should().BeEmpty();


        }
        private async Task<(Product product, ProductVariant variant)> CreateProductWithVariant(AppDbContext context)
        {
            var brand = new Brand
            {
                BrandName = "Nike"
            };

            var category = new Category
            {
                CategoryName = "Футболки",
                IsActive = true
            };

            var color = new Color
            {
                ColorName = "Черный"
            };

            var size = new Size
            {
                SizeName = "M"
            };

            context.Brands.Add(brand);
            context.Categories.Add(category);
            context.Colors.Add(color);
            context.Sizes.Add(size);

            await context.SaveChangesAsync();

            var product = new Product
            {
                ProductName = "Футболка",
                WholesalePrice = 1200,
                IsActive = true,
                BrandId = brand.BrandId,
                CategoryId = category.CategoryId
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            var variant = new ProductVariant
            {
                ProductId = product.ProductId,
                ColorId = color.ColorId,
                SizeId = size.SizeId,
                Quantity = 10,
                MinOrderQuantity = 2
            };

            context.ProductVariants.Add(variant);
            await context.SaveChangesAsync();

            return (product, variant);
        }


        [Fact]
        public async Task CartService_DeleteVariantFromCart_WhenVariantExists()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant) ;

            await data.dbContext.SaveChangesAsync();


            //Act
            await data.cartService.DeleteVariantFromCart(cart.cart.CartId.ToString(), product.variant.ProductVariantId);

            //Assert
            data.dbContext.Carts.First().Items.Should().BeEmpty();
        }

        [Fact]
        public async Task CartService_DeleteVariantFromCart_WhenVariantNotExists()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            await data.dbContext.SaveChangesAsync();

            var randomVariantId = new Random().Next();

            //Act
            await data.cartService.DeleteVariantFromCart(cart.cart.CartId.ToString(), randomVariantId);

            //Assert
            data.dbContext.Carts.First().Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task CartService_GetCartItems_ReturnAllCartItems()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = new Cart() { Items = new List<CartItem> { new() { ProductVariant = product.variant, Quantity = 5 } } };

            data.dbContext.Carts.Add(cart);
            await data.dbContext.SaveChangesAsync();


            //Act
            var cartItems = await data.cartService.GetCartItems(cart.CartId.ToString());

            //Assert
            cartItems.Should().HaveCount(1);
            cartItems[0].CartId.Should().Be(cart.CartId);
            cartItems[0].ProductVariant.Should().NotBeNull();
            cartItems[0].ProductVariant.Product.Should().NotBeNull();
            cartItems[0].ProductVariant.Size.Should().NotBeNull();
            cartItems[0].ProductVariant.Color.Should().NotBeNull();
        }


        [Fact]
        public async Task CartService_GetCartItemById_ReturnCartItem()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            //Act
            var result = await data.cartService.GetCartItemById(cart.cartItem.CartItemId);

            //Assert
            result.Should().NotBeNull();
            result.CartItemId.Should().Be(cart.cartItem.CartItemId);


        }

        private async Task<(Cart cart, CartItem cartItem)> CreateCartWithCartItem(AppDbContext dbContext, Product product, ProductVariant variant)
        {
            var cartItem = new CartItem() { ProductVariant = variant, Quantity = 5 };
            var cart = new Cart() { Items = new List<CartItem> { cartItem } };

            dbContext.Carts.Add(cart);
            await dbContext.SaveChangesAsync();

            return (cart, cartItem);
        }

        [Fact]
        public async Task CartService_UpdateCartItem()
        {

            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            //Act
            await data.cartService.UpdateCartItem(cart.cartItem, 9);

            //Assert
            data.dbContext.Carts.First().Items.First().Quantity.Should().Be(9);
        }

        [Fact]
        public async Task CartService_ClearCart_WhereCartExists()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };

            cart.cart.User = user;

            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.ClearCart(user.Id);

            //Assert
            data.dbContext.Carts.First().Items.Should().BeEmpty();

        }

        [Fact]
        public async Task CartService_CartToUser()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };

            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.CartToUser(cart.cart.CartId.ToString(), user.Id);

            //Assert
            data.dbContext.Carts.First().UserId.Should().NotBeNull();
            data.dbContext.Carts.First().UserId.Should().Be(user.Id);


        }


        [Fact]
        public async Task CartService_GetCartByUser_ReturnCart()
        {
            //Arrange
            var data = await GetCartService();


            var product = await CreateProductWithVariant(data.dbContext);
            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };
            cart.cart.User = user;

            await data.dbContext.SaveChangesAsync();

            //Act
            var result = await data.cartService.GetCartByUser(user.Id);

            //Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(user.Id);
            result.Items.Should().NotBeNull();

        }

        [Fact]
        public async Task CartService_CreateOrderWithItems_ReturnOrder()
        {
            //Arrange
            var data = await GetCartService();

            var order = await CreateOrderWithOrderItem(data.dbContext);

            //Act
            var result = await data.cartService.CreateOrderWithItems(order.order, order.orderItems);

            //Assert
            result.Should().NotBeNull();
            result.OrderItems.Should().NotBeNull();
            result.OrderItems.Should().HaveCount(1);


        }

        private async Task<(Order order, List<OrderItem> orderItems)> CreateOrderWithOrderItem(AppDbContext dbContext)
        {
            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var order = new Order() { UserId = user.Id, TotalAmount = 1200, DeliveryAddress = "Иркутск", };

            var product = await CreateProductWithVariant(dbContext);
            var orderItems = new List<OrderItem> { new OrderItem()
                                                    {
                                                     VariantId = product.variant.ProductVariantId,
                                                    Quantity = 2,
                                                   TotalPrice = product.product.WholesalePrice * 2 } };


            return (order, orderItems);
        }

        [Fact]
        public async Task CartService_GetOrderById_ReturnOrder()
        {
            //Arrange
            var data = await GetCartService();

            var order = await CreateOrderWithOrderItem(data.dbContext);

            order.order.OrderItems = order.orderItems;

            data.dbContext.OrderItems.AddRange(order.orderItems);
            data.dbContext.Orders.Add(order.order);
            await data.dbContext.SaveChangesAsync();

            //Act
            var result = await data.cartService.GetOrderById(order.order.OrderId);

            //Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(order.order.OrderId);

        }

        [Fact]
        public async Task CartService_UpdateOrderStatus()
        {
            //Arrange
            var data = await GetCartService();

            var order = await CreateOrderWithOrderItem(data.dbContext);

            order.order.OrderItems = order.orderItems;

            data.dbContext.Orders.Add(order.order);

            data.dbContext.OrderItems.AddRange(order.orderItems);
            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.UpdateOrderStatus(order.order, OrderStatus.Processing);

            //Assert
            data.dbContext.Orders.First().OrderStatus.Should().Be(OrderStatus.Processing);

        }


        [Fact]
        public async Task CartService_UpdateProductVariantQuantity_WhenOrderPaid()
        {
            //Arrange
            var data = await GetCartService();

            var order = await CreateOrderWithOrderItem(data.dbContext);

            order.order.OrderItems = order.orderItems;

            data.dbContext.OrderItems.AddRange(order.orderItems);
            data.dbContext.Orders.Add(order.order);
            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.UpdateProductVariantQuantity(order.order.OrderId);

            //Assert
            data.dbContext.OrderItems.First().Variant.Quantity.Should().Be(8);

        }


        [Fact]
        public async Task CartService_GetOrders_ReturnAllOrderByUser()
        {
            //Arrange
            var data = await GetCartService();

            var order = await CreateOrderWithOrderItem(data.dbContext);

            order.order.OrderItems = order.orderItems;

            data.dbContext.OrderItems.AddRange(order.orderItems);
            data.dbContext.Orders.Add(order.order);
            await data.dbContext.SaveChangesAsync();

            //Act
            var orders = await data.cartService.GetOrders(order.order.User.Id);

            //Assert
            orders.Should().NotBeNull();
            orders.Should().OnlyContain(x => x.UserId == order.order.UserId);

        }

        [Fact]
        public async Task CartService_MergeCarts_WhenUserCartNotExists()
        {
            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };
            data.dbContext.Users.Add(user);
            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.MergeCarts(cart.cart.CartId.ToString(), user.Id);

            //Assert
            data.dbContext.Carts.First().UserId.Should().Be(user.Id);
        }

        [Fact]
        public async Task CartService_MergeCarts_WhenUserCartExists()
        {

            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };

            data.dbContext.Users.Add(user);
            var userCart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);
            userCart.cart.UserId = user.Id;
            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.MergeCarts(cart.cart.CartId.ToString(), user.Id);

            //Assert
            var checkCart = data.dbContext.Carts.First(c => c.UserId == user.Id);
            checkCart.UserId.Should().Be(user.Id);
            data.dbContext.Carts.Should().NotContain(cart.cart);
            data.dbContext.Carts.Should().HaveCount(1);
            checkCart.Items.Should().HaveCount(1);
            checkCart.Items.First().Quantity.Should().Be(10);

        }

        [Fact]
        public async Task CartService_MergeCarts_ShouldLimitQuantityToAvailableStock()
        {

            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };

            data.dbContext.Users.Add(user);
            var userCart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);
            userCart.cart.UserId = user.Id;

            userCart.cart.Items.First().Quantity += 2;
            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.MergeCarts(cart.cart.CartId.ToString(), user.Id);

            //Assert
            var checkCart = data.dbContext.Carts.First(c => c.UserId == user.Id);
            checkCart.Items.Should().HaveCount(1);
            checkCart.Items.First().Quantity.Should().Be(10);

        }

        [Fact]
        public async Task CartService_MergeCarts_ShouldUnionCart()
        {

            //Arrange
            var data = await GetCartService();

            var product = await CreateProductWithVariant(data.dbContext);

            var cart = await CreateCartWithCartItem(data.dbContext, product.product, product.variant);

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };

            data.dbContext.Users.Add(user);

            var secondVariant = new ProductVariant
            {
                ProductId = product.product.ProductId,
                ColorId = product.variant.ColorId,
                SizeId = product.variant.SizeId,
                Quantity = 67,
                MinOrderQuantity = 10
            };

            data.dbContext.ProductVariants.Add(secondVariant);

            var userCart = new Cart() { UserId = user.Id };
            data.dbContext.Carts.Add(userCart);

            var items = new List<CartItem>() { new() { ProductVariantId = secondVariant.ProductVariantId, Quantity = 15, } };
            data.dbContext.CartItems.AddRange(items);

            userCart.Items = items;

            await data.dbContext.SaveChangesAsync();

            //Act
            await data.cartService.MergeCarts(cart.cart.CartId.ToString(), user.Id);

            //Assert
            data.dbContext.Carts.First(c => c.UserId == user.Id).Items.Should().HaveCount(2);


        }

        [Fact]
        public async Task CartService_MergeCarts_WhereCartIsNull()
        {
            //Arrange
            var data = await GetCartService();

            var user = new User() { Email = "riri@hhf.com", PhoneNumber = "89647387794" };

            data.dbContext.Users.Add(user);

            //Act
            await data.cartService.MergeCarts(null, user.Id);

            //Assert
            data.dbContext.Carts.Should().HaveCount(0);
        }
    }
}
