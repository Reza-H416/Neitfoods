# NutShop - Premium Dry Fruits & Nuts E-Commerce

A modern ASP.NET Core MVC web application for selling premium dry fruits and nuts online.

## Features

- Product Management: Browse and search products by category
- Shopping Cart: Add/remove items with quantity management
- Checkout System: Complete order processing
- Inventory Management: Real-time stock tracking
- Admin Dashboard: Manage products and orders
- Responsive Design: Mobile-friendly interface
- Session Management: User cart persistence

## Project Structure

- Controllers/: MVC Controllers (Home, Products, Cart, Checkout, Admin)
- Models/: Data Models (Product, Category, CartItem, Order, OrderItem)
- Views/: Razor Views (Home, Products, Cart, Checkout, Admin, Shared)
- Data/: Database Context (ApplicationDbContext)
- wwwroot/: Static Files (css, js, images)

## Technology Stack

- Framework: ASP.NET Core 8.0
- Database: SQLite with Entity Framework Core
- Frontend: HTML5, CSS3, JavaScript
- Architecture: MVC Pattern

## Getting Started

1. Restore dependencies:
   dotnet restore

2. Run the application:
   dotnet run

3. Access at: https://localhost:5001

## Database

The app uses SQLite. Database is created automatically on first run.

## Default Products

- Nuts: Almonds, Cashews, Walnuts, Pistachios
- Dry Fruits: Dates, Apricots, Raisins, Figs
- Seeds: Pumpkin Seeds, Sunflower Seeds

## Future Enhancements

- User authentication & registration
- Payment gateway integration
- Order tracking system
- Customer reviews & ratings
- Wishlist functionality
- Advanced search & filters
- Email notifications
- Admin analytics dashboard

---
Created  for fresh, healthy livingwith 
