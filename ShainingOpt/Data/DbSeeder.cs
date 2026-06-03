using Microsoft.AspNetCore.Identity;
using ShainingOpt.DataBase;
using ShainingOpt.Models;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        if (context.Users.Any())
            return;
        #region Users


        // роли
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin" });

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "User" });

        if (!await roleManager.RoleExistsAsync("Manager"))
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Manager" });

        // пользователь admin
        var admin = await userManager.FindByEmailAsync("admin@test.com");

        if (admin == null)
        {
            admin = new User
            {
                UserName = "admin",
                Email = "admin@test.com",
                EmailConfirmed = true,
                PhoneNumber = "89647397703"
            };

            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // менеджер
        var manager = await userManager.FindByEmailAsync("manager@test.com");

        if (manager == null)
        {
            manager = new User
            {
                UserName = "manager",
                Email = "manager@test.com",
                EmailConfirmed = true,
                 PhoneNumber = "89667397703"
            };

            await userManager.CreateAsync(manager, "Manager123!");
            await userManager.AddToRoleAsync(manager, "Manager");
        }

        // обычный пользователь
        var user = await userManager.FindByEmailAsync("user@test.com");

        if (user == null)
        {
            user = new User
            {
                UserName = "user",
                Email = "user@test.com",
                EmailConfirmed = true,
                PhoneNumber = "89647390003",
                Company = new Company
                {
                    CompanyName = "Тест название",
                    Inn = "1234567890",
                    Kpp = "123456789",
                    LegalAddress = "Тест адрес",
                    ContactPerson = "Иванов Иван Иванович"

                }
            };

            await userManager.CreateAsync(user, "User123!");
            await userManager.AddToRoleAsync(user, "User");
        }

        #endregion

        #region Brands
        if (context.Brands.Any())
            return;

        var brands = new List<Brand>
    {
        new() { BrandName = "Nike" },
        new() { BrandName = "Adidas" },
        new() { BrandName = "Puma" },
        new() { BrandName = "Reebok" },
        new() { BrandName = "New Balance" },
        new() { BrandName = "Asics" },
        new() { BrandName = "Under Armour" },
        new() { BrandName = "Columbia" },
        new() { BrandName = "The North Face" },
        new() { BrandName = "Zara" }
    };

        context.Brands.AddRange(brands);

        #endregion

        #region Categories
        if (context.Categories.Any())
            return;

        var categories = new List<Category>
    {
        new() { CategoryName = "Футболки" },
        new() { CategoryName = "Шарфы" },
        new() { CategoryName = "Обувь" },
        new() { CategoryName = "Сумки" },
        new() { CategoryName = "Головные уборы" },
        new() { CategoryName = "Ремни" },
        new() { CategoryName = "Кошельки" },
        new() { CategoryName = "Украшения" },
        new() { CategoryName = "Часы" },
        new() { CategoryName = "Очки" },
        new() { CategoryName = "Худи и свитшоты" }
    };

        context.Categories.AddRange(categories);

        #endregion

        #region Colors

        var colors = new List<Color>
    {
        new() { ColorName = "Красный" },
        new() { ColorName = "Синий" },
        new() { ColorName = "Зеленый" },
        new() { ColorName = "Черный" },
        new() { ColorName = "Белый" },
        new() { ColorName = "Желтый" },
        new() { ColorName = "Розовый" },
        new() { ColorName = "Фиолетовый" },
        new() { ColorName = "Оранжевый" },
        new() { ColorName = "Серый" },
        new() { ColorName = "Коричневый" }
    };

        context.Colors.AddRange(colors);

        #endregion

        #region Sizes

        var sizes = new List<Size>
    {
        new() { SizeName = "XS" },
        new() { SizeName = "S" },
        new() { SizeName = "M" },
        new() { SizeName = "L" },
        new() { SizeName = "XL" },
        new() { SizeName = "XXL" },
        new() { SizeName = "36" },
        new() { SizeName = "38" },
        new() { SizeName = "40" },
        new() { SizeName = "42" },
        new() { SizeName = "Без размера" }
    };

        context.Sizes.AddRange(sizes);

        await context.SaveChangesAsync();

        #endregion

        #region Products
        if (context.Products.Any())
            return;

        var products = new List<Product>
    {
        new()
        {
            ProductName = "Базовая футболка",
            WholesalePrice = 1000,
            CategoryId = 1,
            BrandId = 1,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/qMW46bD0/f1g1gwoinlcysgkcnb9d98uld9nc9ulh.webp",
            Description = "Базовая футболка из хлопка."
        },
        new()
        {
            ProductName = "Худи FINNTRAIL Buggy Khaki",
            WholesalePrice = 1500,
            CategoryId = 11,
            BrandId = 2,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/cSyjw8gL/image.png",
            Description = "Теплое худи с начесом."
        },
        new()
        {
            ProductName = "Шарф шерстяной",
            WholesalePrice = 700,
            CategoryId = 2,
            BrandId = 1,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/r2RSvxy0/image.png",
            Description = "Теплый зимний шарф."
        },
        new()
        {
            ProductName = "Кроссовки",
            WholesalePrice = 1200,
            CategoryId = 3,
            BrandId = 3,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/39txvJgG/image.png",
            Description = "Легкие спортивные кроссовки."
        },
        new()
        {
            ProductName = "Рюкзак городской",
            WholesalePrice = 750,
            CategoryId = 4,
            BrandId = 2,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/60WfX3Qm/image.png",
            Description = "Универсальный городской рюкзак."
        },
        new()
        {
            ProductName = "Кепка",
            WholesalePrice = 350,
            CategoryId = 5,
            BrandId = 1,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/ycKNhjgL/image.png",
            Description = "Легкая летняя кепка."
        },
        new()
        {
            ProductName = "Ремень кожаный",
            WholesalePrice = 499,
            CategoryId = 6,
            BrandId = 2,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/SDXmstrX/image.png",
            Description = "Классический кожаный ремень."
        },
        new()
        {
            ProductName = "Кошелёк",
            WholesalePrice = 400,
            CategoryId = 7,
            BrandId = 3,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/fzV8n9h7/image.png",
            Description = "Вместительный кошелек."
        },
        new()
        {
            ProductName = "Браслет",
            WholesalePrice = 2000,
            CategoryId = 8,
            BrandId = 1,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/5xxStdHy/image.png",
            Description = "Стильный браслет."
        },
        new()
        {
            ProductName = "Наручные часы",
            WholesalePrice = 2500,
            CategoryId = 9,
            BrandId = 2,
            IsActive = true,
            MainImageUrl = "https://i.ibb.co/pvMsfB63/image.png",
            Description = "Классические часы."
        }
    };

        context.Products.AddRange(products);

        await context.SaveChangesAsync();

        #endregion

        #region ProductVariants

        var variants = new List<ProductVariant>
{
// Базовая футболка
new()
{
ProductId = products[0].ProductId,
ColorId = 5, // Белый
SizeId = 3, // M
Quantity = 20,
ImageUrl = "https://i.ibb.co/qMW46bD0/f1g1gwoinlcysgkcnb9d98uld9nc9ulh.webp"
},
new()
{
ProductId = products[0].ProductId,
ColorId = 4, // Черный
SizeId = 4, // L
Quantity = 15,
ImageUrl = "https://i.ibb.co/dwmzbjf8/angbmrggv68sbfes9mpu4sj4hrjrfin1.webp"
},

// Худи
new()
{
    ProductId = products[1].ProductId,
    ColorId = 10, // Серый
    SizeId = 5, // XL
    Quantity = 10,   
    ImageUrl = "https://i.ibb.co/b0XDp6B/image.png"
},
new()
{
    ProductId = products[1].ProductId,
    ColorId = 4, // Черный
    SizeId = 4, // L
    Quantity = 12,
    ImageUrl = "https://i.ibb.co/S7d2PfN6/image.png"
},

// Шарф
new()
{
    ProductId = products[2].ProductId,
    ColorId = 11, // Коричневый
    SizeId = 11, // Без размера
    Quantity = 30,
    ImageUrl ="https://i.ibb.co/r2RSvxy0/image.png"
},

// Кроссовки
new()
{
    ProductId = products[3].ProductId,
    ColorId = 4, // Черный
    SizeId = 7, // 36
    Quantity = 8,
    ImageUrl = "https://i.ibb.co/39txvJgG/image.png"
},
new()
{
    ProductId = products[3].ProductId,
    ColorId = 4,
    SizeId = 8, // 38
    Quantity = 12
},
new()
{
    ProductId = products[3].ProductId,
    ColorId = 4,
    SizeId = 9, // 40
    Quantity = 10
},

// Рюкзак
new()
{
    ProductId = products[4].ProductId,
    ColorId = 4, // Черный
    SizeId = 11,
    Quantity = 25,
    ImageUrl = "https://i.ibb.co/60WfX3Qm/image.png"
},

// Кепка
new()
{
    ProductId = products[5].ProductId,
    ColorId = 4, // Черный
    SizeId = 11,
    Quantity = 18,
    ImageUrl = "https://i.ibb.co/ycKNhjgL/image.png"
},

// Ремень
new()
{
    ProductId = products[6].ProductId,
    ColorId = 4, // Черный
    SizeId = 11,
    Quantity = 20,
    ImageUrl = "https://i.ibb.co/rRVhk2g8/42036448.webp"
},

// Кошелек
new()
{
    ProductId = products[7].ProductId,
    ColorId = 4,// Черный
    SizeId = 11,
    Quantity = 15,
    ImageUrl = "https://i.ibb.co/fzV8n9h7/image.png"
},

// Браслет
new()
{
    ProductId = products[8].ProductId,
    ColorId = 10, //Серый
    SizeId = 11,
    Quantity = 12,
    ImageUrl ="https://i.ibb.co/5xxStdHy/image.png"
},

// Часы
new()
{
    ProductId = products[9].ProductId,
    ColorId = 4,
    SizeId = 11,
    Quantity = 7,
    ImageUrl = "https://i.ibb.co/fzV8n9h7/image.png"
}

};

        context.ProductVariants.AddRange(variants);

        await context.SaveChangesAsync();

        #endregion

    }

}
