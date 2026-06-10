using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShainingOpt.UnitTests.Services
{
    public class CatalogServiceTest
    {

        private async Task<AppDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new AppDbContext(options);

            databaseContext.Database.EnsureCreated();
           
            return databaseContext;

        }

        [Fact]
        public async Task CatalogService_GetProducts_ReturnsActiveProducts()
        {

            var data = await GetCatalogService();

            //Arrange
            data.dbContext.Products.AddRange(
                  new Product()
                  {
                      Brand = new Brand() { BrandName = "Adaa" },
                      Category = new Category() { CategoryName = "Футболка", IsActive = true },
                      IsActive = true,
                      ProductName = "Удобная футболка",
                      WholesalePrice = 1200,

                  },
                     new Product()
                     {
                         Brand = new Brand() { BrandName = "Adaa" },
                         Category = new Category() { CategoryName = "Майка", IsActive = true },
                         IsActive = false,
                         ProductName = "Удобная майка",
                         WholesalePrice = 1200,

                     }
                  );
            
            await data.dbContext.SaveChangesAsync();
            

            //Act
            var result = await data.catalogService.GetProducts();

            //Assert
            result.Should().HaveCount(1);
            result.First().Brand.Should().NotBeNull();
            result.First().Category.Should().NotBeNull();
            result.Should().OnlyContain(p => p.IsActive);
            result.First().ProductName.Should().Be("Удобная футболка");

        }

        [Fact]
        public async Task CatalogService_GetProducts_ReturnEmptyList()
        {

            //Arrange
            var data = await GetCatalogService();

            //Act
            var result = await data.catalogService.GetProducts();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
          
        }

        [Fact]
        public async Task CatalogService_GetBrands_ReturnAllBrands()
        {
            //Arrange
            var data = await GetCatalogService();

            var brands = new List<Brand>
    {
        new() { BrandName = "Nike" },
        new() { BrandName = "Adidas" }
    };
            data.dbContext.Brands.AddRange(brands);
            await data.dbContext.SaveChangesAsync();

            //Act
            var result = await data.catalogService.GetBrands();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);

        }

        [Fact]
        public async Task CatalogService_GetSizes_ReturnAllSizes()
        {
            //Arrange
            var data = await GetCatalogService();

            var sizes = new List<Size>
    {
        new() { SizeName = "S" },
        new() { SizeName = "M" },
        new() { SizeName = "L" }
    };
            data.dbContext.Sizes.AddRange(sizes);
            await data.dbContext.SaveChangesAsync();

            //Act
            var result = await data.catalogService.GetSizes();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(3);

        }

        [Fact]
        public async Task CatalogService_GetColors_ReturnAllColors()
        {
            //Arrange
            var data = await GetCatalogService();
            var colors = new List<Color>
    {
        new() { ColorName = "Черный" },
        new() { ColorName = "Белый" }
    };
            data.dbContext.Colors.AddRange(colors);
            await data.dbContext.SaveChangesAsync();

            //Act
            var result = await data.catalogService.GetColors();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);

        }

        [Fact]
        public async Task CatalogService_GetCaterories_ReturnActiveCaterories()
        {
            //Arrange
            var data = await GetCatalogService();
            var categories = new List<Category>
    {
        new() { CategoryName = "Футболки", IsActive = true },
        new() { CategoryName = "Обувь", IsActive = true },
         new() { CategoryName = "Майка", IsActive = false },
    };
            data.dbContext.Categories.AddRange(categories);
            await data.dbContext.SaveChangesAsync();

            //Act
            var result = await data.catalogService.GetCaterories();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(c => c.IsActive);

           
        }


        [Fact]
        public async Task CatalogService_GetProductWithVariants_ReturnProductWithVariants()
        {

            //Arrange
            var data = await GetCatalogService();

            var product = await CreateProductWithVariant(data.dbContext);

            //Act
            var result = await data.catalogService.GetProductWithVariants(product.product.ProductId);

            //Assert
            result.Should().BeOfType<Product>();
            result.ProductVariants.Should().NotBeNull();
            result.ProductVariants.Should().HaveCount(1);

        }


        [Fact]
        public async Task CatalogService_GetDefaultVariant_ReturnDefaultVariant()
        {
            //Arrange
            var data = await GetCatalogService();
            var product = await CreateProductWithVariant(data.dbContext);

            //Act
            var result = await data.catalogService.GetDefaultVariant(product.product.ProductId);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ProductVariant>();
            result.ProductId.Should().Be(product.product.ProductId);
            result.Color.Should().NotBeNull();
            result.Size.Should().NotBeNull();
            result.Quantity.Should().Be(10);

        }



        [Fact]
        public async Task CatalogService_GetProductVariant_ReturnProductVariant()
        {
            //Arrange
            var data = await GetCatalogService();

            var products = await CreateProductWithVariant(data.dbContext);

            //Act
            var result = await data.catalogService.GetProductVariant(products.variant.ProductVariantId);


            //Assert
            result.Should().BeOfType<ProductVariant>();
            result.Color.Should().NotBeNull();
            result.Size.Should().NotBeNull();
            result.ProductVariantId.Should().Be(products.variant.ProductVariantId);
        }


        [Theory]
        [InlineData("", 3)]
        [InlineData("nike", 2)]
        [InlineData("фут", 1)]
        [InlineData("samsung", 0)]
        public async Task CatalogService_GetProductsWithSearch_ReturnProductsWithSearch(string value, int expected)
        {
            var dbContext = await GetDbContext();

            var brandNike = new Brand
            {
                BrandName = "Nike"
            };
            var brandAdidas = new Brand
            {
                BrandName = "Adidas"
            };

            dbContext.Brands.AddRange(brandNike, brandAdidas);

            var categoryTshirt = new Category
            {
                CategoryName = "Футболки",
                IsActive = true
            };
            var categoryBoots = new Category
            {
                CategoryName = "Ботинки",
                IsActive = true
            };

            dbContext.Categories.AddRange(categoryBoots, categoryTshirt);

            //Arrange
            dbContext.Products.AddRange(
                  new Product()
                  {
                      Brand = brandNike,
                      Category = categoryTshirt,
                      IsActive = true,
                      ProductName = "Удобная футболка",
                      WholesalePrice = 1200,

                  },
                     new Product()
                     {
                         Brand = brandNike,
                         Category = categoryTshirt,
                         IsActive = true,
                         ProductName = "Удобная майка",
                         WholesalePrice = 1200,

                     },
                     new Product()
                     {
                         Brand = brandAdidas,
                         Category = categoryBoots,
                         IsActive = true,
                         ProductName = "Зимнии ботинки",
                         WholesalePrice = 1200,

                     }
                  );

            await dbContext.SaveChangesAsync();
            var catalogService = new CatalogService(dbContext);

            //Act
            var result = await catalogService.GetProductsWithSearch(value);

            //Assert
            result.Count.Should().Be(expected);
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


        private async Task<(AppDbContext dbContext, CatalogService catalogService)> GetCatalogService()
        {
            var dbContext = await GetDbContext();
            var catalogService = new CatalogService(dbContext);
            return (dbContext, catalogService);
        }
    }
}
