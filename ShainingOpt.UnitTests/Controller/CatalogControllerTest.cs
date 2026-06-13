using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ShainingOpt.Controllers;
using ShainingOpt.Models;
using ShainingOpt.Services.Interfaces;
using ShainingOpt.ViewModels.Cart;
using ShainingOpt.ViewModels.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShainingOpt.UnitTests.Controller
{
    public class CatalogControllerTest
    {
        private readonly ICatalogService _catalogService;
        private readonly CatalogController _catalogController;

        public CatalogControllerTest()
        {
            _catalogService = A.Fake<ICatalogService>();
            _catalogController = new CatalogController(_catalogService);
        }

        [Fact]
        public async Task CatalogControllerTest_Product_WhereProductIsNull_ReturnNotFound()
        {
            A.CallTo(() => _catalogService.GetProductWithVariants(A<int>._)).Returns((Product?)null);

            var result = await _catalogController.Product(1, null);

            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();

        }


        [Fact]
        public async Task CatalogControllerTest_Product_WhereVariantIsNull_ReturnNotFound()
        {
            var product = new Product();
            A.CallTo(() => _catalogService.GetProductWithVariants(A<int>._)).Returns(product);
            A.CallTo(() => _catalogService.GetDefaultVariant(A<int>._))
       .Returns((ProductVariant?)null);

            var result = await _catalogController.Product(1, null);

            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();

        }

        [Fact]
        public async Task CatalogControllerTest_Product_ReturnProductView()
        {
            var product = new Product
            {
                ProductId = 1,
                ProductName = "Футболка",
                WholesalePrice = 1200,
                Brand = new Brand
                {
                    BrandName = "Nike"
                },
                ProductVariants = new List<ProductVariant>()
            };

            var variant = new ProductVariant
            {
                ProductVariantId = 1,
                ColorId = 1,
                SizeId = 1,
                Quantity = 10,
                MinOrderQuantity = 2,
                Color = new Color
                {
                    ColorName = "Черный"
                },
                Size = new Size
                {
                    SizeName = "M"
                }
            };

            product.ProductVariants.Add(variant);
            A.CallTo(() => _catalogService.GetProductWithVariants(A<int>._)).Returns(product);
            A.CallTo(() => _catalogService.GetProductVariant(A<int>._)).Returns(variant);

            var result = await _catalogController.Product(product.ProductId, variant.ProductVariantId);
           
            result.Should().BeOfType<ViewResult>();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Model.Should().BeOfType<ProductViewModel>();

        }


    }
}
