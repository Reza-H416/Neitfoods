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
        var nutsCategorySeed = new NutShop.Models.Category
        {
            Name = "Nuts",
            Description = "Premium quality nuts"
        };

        var dryFruitsCategorySeed = new NutShop.Models.Category
        {
            Name = "Dried Fruits",
            Description = "Natural dried fruits"
        };

        context.Categories.AddRange(nutsCategorySeed, dryFruitsCategorySeed);
        context.SaveChanges();
    }

    var nutsCategory = context.Categories.First(c => c.Name == "Nuts");
    var dryFruitsCategory = context.Categories.First(c => c.Name == "Dried Fruits");

    var seedProducts = new List<NutShop.Models.Product>
    {
        new NutShop.Models.Product
        {
            Name = "Almonds",
            Description = "Premium quality almonds, perfect for healthy snacks.",
            Price = 89,
            CategoryId = nutsCategory.Id,
            StockQuantity = 50,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Almonds"
        },
        new NutShop.Models.Product
        {
            Name = "Cashew Nuts",
            Description = "Soft and tasty cashew nuts with rich flavour.",
            Price = 99,
            CategoryId = nutsCategory.Id,
            StockQuantity = 40,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Cashews"
        },
        new NutShop.Models.Product
        {
            Name = "Dates",
            Description = "Sweet and natural dried dates.",
            Price = 79,
            CategoryId = dryFruitsCategory.Id,
            StockQuantity = 60,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Dates"
        },
        new NutShop.Models.Product
        {
            Name = "Walnuts",
            Description = "Rich and nutritious walnuts, excellent for brain health.",
            Price = 109,
            CategoryId = nutsCategory.Id,
            StockQuantity = 35,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Walnuts"
        },
        new NutShop.Models.Product
        {
            Name = "Pistachios",
            Description = "Delicious green pistachios, naturally salted.",
            Price = 129,
            CategoryId = nutsCategory.Id,
            StockQuantity = 30,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Pistachios"
        },
        new NutShop.Models.Product
        {
            Name = "Raisins",
            Description = "Sweet dried raisins, perfect for snacking and baking.",
            Price = 69,
            CategoryId = dryFruitsCategory.Id,
            StockQuantity = 75,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Raisins"
        },
        new NutShop.Models.Product
        {
            Name = "Dried Apricots",
            Description = "Golden dried apricots with natural sweetness.",
            Price = 85,
            CategoryId = dryFruitsCategory.Id,
            StockQuantity = 45,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Apricots"
        },
        new NutShop.Models.Product
        {
            Name = "Figs",
            Description = "Premium quality dried figs, rich in fiber.",
            Price = 99,
            CategoryId = dryFruitsCategory.Id,
            StockQuantity = 40,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Figs"
        },
        new NutShop.Models.Product
        {
            Name = "Hazelnuts",
            Description = "Premium roasted hazelnuts, perfect for chocolate lovers.",
            Price = 119,
            CategoryId = nutsCategory.Id,
            StockQuantity = 25,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Hazelnuts"
        },
        new NutShop.Models.Product
        {
            Name = "Pecans",
            Description = "Large and buttery pecans, perfect for desserts.",
            Price = 139,
            CategoryId = nutsCategory.Id,
            StockQuantity = 20,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Pecans"
        },
        new NutShop.Models.Product
        {
            Name = "Brazil Nuts",
            Description = "Large Brazil nuts packed with selenium and healthy fats.",
            Price = 125,
            CategoryId = nutsCategory.Id,
            StockQuantity = 28,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Brazil+Nuts"
        },
        new NutShop.Models.Product
        {
            Name = "Macadamia Nuts",
            Description = "Buttery macadamia nuts with a rich, creamy texture.",
            Price = 149,
            CategoryId = nutsCategory.Id,
            StockQuantity = 22,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Macadamia"
        },
        new NutShop.Models.Product
        {
            Name = "Dried Cranberries",
            Description = "Tangy sweet dried cranberries for snacking and salads.",
            Price = 74,
            CategoryId = dryFruitsCategory.Id,
            StockQuantity = 55,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Cranberries"
        },
        new NutShop.Models.Product
        {
            Name = "Prunes",
            Description = "Naturally sweet prunes with soft texture and high fiber.",
            Price = 82,
            CategoryId = dryFruitsCategory.Id,
            StockQuantity = 48,
            CreatedAt = DateTime.Now,
            ImageUrl = "https://via.placeholder.com/250x200?text=Prunes"
        }
    };

    var existingNames = context.Products.Select(p => p.Name).ToHashSet();
    var productsToInsert = seedProducts.Where(p => !existingNames.Contains(p.Name)).ToList();

    if (productsToInsert.Any())
    {
        context.Products.AddRange(productsToInsert);
        context.SaveChanges();
    }
}

app.Run();
