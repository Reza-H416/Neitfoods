using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NutShop.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=nutshop.db"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NutShop.Data.ApplicationDbContext>();

    context.Database.EnsureCreated();

    if (!context.Categories.Any())
    {
        var nutsCategory = new NutShop.Models.Category
        {
            Name = "Nuts",
            Description = "Premium quality nuts"
        };

        var dryFruitsCategory = new NutShop.Models.Category
        {
            Name = "Dried Fruits",
            Description = "Natural dried fruits"
        };

        context.Categories.AddRange(nutsCategory, dryFruitsCategory);
        context.SaveChanges();
    }

    if (!context.Products.Any())
    {
        var nutsCategory = context.Categories.First(c => c.Name == "Nuts");
        var dryFruitsCategory = context.Categories.First(c => c.Name == "Dried Fruits");

        context.Products.AddRange(
            new NutShop.Models.Product
            {
                Name = "Almonds",
                Description = "Premium quality almonds, perfect for healthy snacks.",
                Price = 89,
                CategoryId = nutsCategory.Id,
                StockQuantity = 50,
                CreatedAt = DateTime.Now
            },
            new NutShop.Models.Product
            {
                Name = "Cashew Nuts",
                Description = "Soft and tasty cashew nuts with rich flavour.",
                Price = 99,
                CategoryId = nutsCategory.Id,
                StockQuantity = 40,
                CreatedAt = DateTime.Now
            },
            new NutShop.Models.Product
            {
                Name = "Dates",
                Description = "Sweet and natural dried dates.",
                Price = 79,
                CategoryId = dryFruitsCategory.Id,
                StockQuantity = 60,
                CreatedAt = DateTime.Now
            }
        );

        context.SaveChanges();
    }
}

app.Run();
