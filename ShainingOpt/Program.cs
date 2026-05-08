using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.ViewModels;


var builder = WebApplication.CreateBuilder(args);


builder.WebHost.UseUrls("http://localhost:5212");



// Add services to the container.
builder.Services.AddControllersWithViews( options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<User, Role>(options =>
{

    // Требовать ли цифры (0-9) в пароле
    // false = можно создавать пароль без цифр
    options.Password.RequireDigit = true;

    // Минимальная длина пароля
    // 6 = пароль должен быть не короче 6 символов
    options.Password.RequiredLength = 6;

    // Требовать ли заглавные буквы (A-Z) в пароле
    // false = можно использовать только строчные буквы
    options.Password.RequireUppercase = true;

    // Требовать ли специальные символы (!,@,#,$,%, etc.) в пароле
    // false = можно обойтись без спецсимволов
    options.Password.RequireNonAlphanumeric = true;

    //options.User.RequireUniqueEmail = true; // требовать уникальный email
    // options.Lockout.MaxFailedAccessAttempts = 5; // блокировка после 5 попыток
    // options.SignIn.RequireConfirmedEmail = true; // требовать подтверждение email
})

// Указываем, что Identity будет использовать Entity Framework
// для хранения данных о пользователях и ролях
.AddEntityFrameworkStores<AppDbContext>()

// Добавляем провайдеры для генерации токенов
// (нужны для сброса пароля, подтверждения email и т.д.)
.AddDefaultTokenProviders();

builder.Services.AddScoped<AccountService>();

builder.Services.Configure<YooKassaSettings>(
    builder.Configuration.GetSection("YooKassa"));
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<EmailService>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/";
});

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    //вытаскиваем RoleManager из ДИ
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

    string[] roles = { "Admin", "Manager", "Client" };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new Role { Name = roleName });
        }
    }
} 
app.UseHttpsRedirection();
app.UseRouting();

// Проверяет, кто делает запрос (определяет личность пользователя)
app.UseAuthentication();
// Проверяет, имеет ли пользователь право на запрашиваемый ресурс
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
