using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.Services;
using ShainingOpt.Services.Configurations;
using ShainingOpt.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews( options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services
    .AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();




builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.Configure<YooKassaSettings>(
    builder.Configuration.GetSection("YooKassa"));
builder.Services.AddScoped<IPaymentService,PaymentService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IEmailService, EmailService>();


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
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    await DbSeeder.SeedAsync(context, userManager, roleManager);
}
//using (var scope = app.Services.CreateScope())
//{
//    //вытаскиваем RoleManager из ДИ
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

//    string[] roles = { "Admin", "Manager", "Client" };

//    foreach (var roleName in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(roleName))
//        {
//            await roleManager.CreateAsync(new Role { Name = roleName });
//        }
//    }
//} 
app.UseHttpsRedirection();
app.UseRouting();

// Проверяет, кто делает запрос (определяет личность пользователя)
app.UseAuthentication();
// Проверяет, имеет ли пользователь право на запрашиваемый ресурс
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
