using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NutShop.Data;
using NutShop.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IAuthService, AuthService>();

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
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
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
                Description = "Premium quality almonds, perfect for healthy snacks and baking.",
                Price = 899,
                CategoryId = nutsCategory.Id,
                StockQuantity = 50,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Almonds"
            },
            new NutShop.Models.Product
            {
                Name = "Cashew Nuts",
                Description = "Soft and tasty cashew nuts with rich flavor. Great for snacking.",
                Price = 999,
                CategoryId = nutsCategory.Id,
                StockQuantity = 40,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Cashews"
            },
            new NutShop.Models.Product
            {
                Name = "Dates",
                Description = "Sweet and natural dried dates. Perfect for energy boost.",
                Price = 799,
                CategoryId = dryFruitsCategory.Id,
                StockQuantity = 60,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Dates"
            },
            new NutShop.Models.Product
            {
                Name = "Walnuts",
                Description = "Rich and nutritious walnuts, excellent for brain health.",
                Price = 1099,
                CategoryId = nutsCategory.Id,
                StockQuantity = 35,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Walnuts"
            },
            new NutShop.Models.Product
            {
                Name = "Pistachios",
                Description = "Delicious green pistachios, naturally salted.",
                Price = 1299,
                CategoryId = nutsCategory.Id,
                StockQuantity = 30,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Pistachios"
            },
            new NutShop.Models.Product
            {
                Name = "Raisins",
                Description = "Sweet dried raisins, perfect for snacking and baking.",
                Price = 699,
                CategoryId = dryFruitsCategory.Id,
                StockQuantity = 75,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Raisins"
            },
            new NutShop.Models.Product
            {
                Name = "Dried Apricots",
                Description = "Golden dried apricots with natural sweetness.",
                Price = 850,
                CategoryId = dryFruitsCategory.Id,
                StockQuantity = 45,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Apricots"
            },
            new NutShop.Models.Product
            {
                Name = "Figs",
                Description = "Premium quality dried figs, rich in fiber.",
                Price = 999,
                CategoryId = dryFruitsCategory.Id,
                StockQuantity = 40,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Figs"
            },
            new NutShop.Models.Product
            {
                Name = "Hazelnuts",
                Description = "Premium roasted hazelnuts, perfect for chocolate lovers.",
                Price = 1199,
                CategoryId = nutsCategory.Id,
                StockQuantity = 25,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Hazelnuts"
            },
            new NutShop.Models.Product
            {
                Name = "Pecans",
                Description = "Large and buttery pecans, perfect for desserts and baking.",
                Price = 1399,
                CategoryId = nutsCategory.Id,
                StockQuantity = 20,
                CreatedAt = DateTime.Now,
                ImageUrl = "https://via.placeholder.com/300x250?text=Pecans"
            }
        );

        context.SaveChanges();
    }

    if (!context.Users.Any(u => u.IsAdmin))
    {
        var adminUser = new NutShop.Models.User
        {
            FullName = "Admin User",
            Email = "admin@neitfoods.com",
            PasswordHash = authService.HashPassword("Admin@123"),
            RegisteredAt = DateTime.Now,
            IsAdmin = true,
            PhoneNumber = "",
            ShippingAddress = ""
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

app.Run();
