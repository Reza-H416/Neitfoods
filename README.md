# NeitFoods NutShop - Complete E-Commerce Platform

A fully functional ASP.NET Core MVC e-commerce website for selling premium nuts and dried fruits with user authentication, product management, shopping cart, and admin features.

## 🎯 Key Features

### Customer Features
✅ **User Authentication**
- User registration with email validation
- Secure login with password hashing (SHA256)
- User profile management
- Session-based authentication

✅ **Shopping Experience**
- Browse all products with professional layout
- Search products by name/description
- Filter by category
- Sort by (Name, Price, Rating, Newest)
- View detailed product information
- Add/remove items from shopping cart
- Quantity management

✅ **Product Reviews**
- Rate products (1-5 stars)
- Write product reviews
- View average rating and all reviews
- See other customers' feedback

✅ **Checkout & Orders**
- Professional checkout flow
- Order summary with shipping & tax
- Tracking number generation
- Order confirmation with details
- Estimated delivery dates

### Admin Features
✅ **Admin Dashboard**
- Total revenue analytics
- Total orders count
- Products inventory
- User statistics
- Recent orders overview

✅ **Product Management**
- Add new products
- Edit existing products
- Delete products
- Upload product images
- Manage stock quantities
- Set prices

✅ **Order Management**
- View all orders
- Order details with items
- Update order status
- Track shipments
- Customer information

## 📁 Project Structure

```
NutShop/
├── Controllers/
│   ├── AccountController.cs       # User registration & login
│   ├── AdminController.cs         # Admin dashboard & management
│   ├── CartController.cs          # Shopping cart operations
│   ├── CheckoutController.cs      # Order processing
│   ├── HomeController.cs          # Home page
│   └── ProductsController.cs      # Product browsing & reviews
├── Models/
│   ├── User.cs                    # User account model
│   ├── Product.cs                 # Product model
│   ├── Category.cs                # Product category
│   ├── CartItem.cs                # Shopping cart item
│   ├── Order.cs                   # Customer order
│   ├── OrderItem.cs               # Order line item
│   └── Review.cs                  # Product review
├── Services/
│   └── AuthService.cs             # Password hashing service
├── Data/
│   └── ApplicationDbContext.cs     # Entity Framework DbContext
├── Views/
│   ├── Account/                   # Login, Register, Profile
│   ├── Admin/                     # Dashboard, Products, Orders
│   ├── Cart/                      # Shopping cart
│   ├── Checkout/                  # Checkout & Confirmation
│   ├── Home/                      # Home page
│   ├── Products/                  # Products listing & details
│   └── Shared/                    # Layout & shared views
├── wwwroot/
│   ├── css/                       # All stylesheets
│   ├── js/                        # JavaScript files
│   └── images/                    # Product images
├── Program.cs                     # Application startup
└── NutShop.csproj                # Project file
```

## 🗄️ Database Schema

### Users Table
- Id (Primary Key)
- FullName
- Email (Unique)
- PasswordHash (SHA256)
- PhoneNumber
- ShippingAddress
- RegisteredAt
- IsAdmin

### Products Table
- Id
- Name
- Description
- Price
- CategoryId (Foreign Key)
- StockQuantity
- ImageUrl
- CreatedAt

### Categories Table
- Id
- Name
- Description

### CartItems Table
- Id
- UserId
- ProductId (Foreign Key)
- Quantity
- UnitPrice
- AddedAt

### Orders Table
- Id
- UserId (Foreign Key)
- OrderDate
- Status
- TotalAmount
- ShippingAddress
- PhoneNumber
- Email
- TrackingNumber
- EstimatedDelivery

### OrderItems Table
- Id
- OrderId (Foreign Key)
- ProductId (Foreign Key)
- Quantity
- UnitPrice
- TotalPrice

### Reviews Table
- Id
- ProductId (Foreign Key)
- UserId (Foreign Key)
- Rating (1-5)
- Comment
- CreatedAt

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 or later
- Visual Studio or VS Code
- Git

### Installation & Running

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd NutShop
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open browser and go to: `https://localhost:5001` (or `http://localhost:5000`)
   - Database will be auto-created on first run

## 👤 Default Credentials

### Admin Account
- **Email:** admin@neitfoods.com
- **Password:** Admin@123

Use these credentials to access the admin dashboard at `/Admin/Dashboard`

## 🛍️ Test Data

The application comes pre-populated with:
- 2 Product Categories (Nuts, Dried Fruits)
- 10 Sample Products with descriptions and prices
- 1 Admin User account

## 💳 Checkout Process

1. **Add to Cart** - Click "Add to Cart" on any product
2. **Review Cart** - View `/Cart` to see items and quantities
3. **Checkout** - Click "Proceed to Checkout"
4. **Shipping Details** - Enter/verify phone and address
5. **Order Confirmation** - View order details with tracking number
6. **Estimated Delivery** - Shows 5 days from order date

## 📊 Admin Dashboard

Access admin panel at `/Admin` (requires admin login):

- **Dashboard** - View key metrics and recent orders
- **Products** - Add, edit, or delete products
- **Orders** - View and manage customer orders
- **Analytics** - See revenue, customer count, and inventory

## 🎨 Features Highlight

### Search & Filtering
- Real-time product search
- Category filtering
- Sorting options (price, rating, newest)

### Product Details
- Star ratings (1-5)
- Customer reviews
- Stock information
- Add review functionality

### User Experience
- Responsive design (mobile-friendly)
- Professional styling with gradients
- Clear call-to-action buttons
- Fast checkout process

### Security
- Password hashing with SHA256
- Session-based authentication
- CSRF protection with Anti-Forgery tokens
- Secure form submissions

## 🔄 Order Status Flow

Orders follow this status progression:
1. **Pending** - Order received, awaiting processing
2. **Processing** - Order is being prepared
3. **Shipped** - Order on the way
4. **Delivered** - Order delivered

Admin can update order status from the Orders management page.

## 💰 Pricing

Sample product prices (in ₹):
- Almonds: ₹899
- Cashews: ₹999
- Dates: ₹799
- Walnuts: ₹1099
- Pistachios: ₹1299
- Raisins: ₹699

## 📱 Responsive Design

The entire website is responsive and works on:
- Desktop (1200px+)
- Tablet (768px - 1199px)
- Mobile (less than 768px)

## 🔐 Authentication & Authorization

- Users must register or login to place orders
- Admin login required for admin dashboard
- Session tokens used for security
- Password stored as SHA256 hash

## 🎯 Future Enhancements

Potential features to add:
- Payment gateway integration (Stripe, PayPal)
- Email notifications for orders
- Wishlist functionality
- Product recommendations
- Discount coupons
- User reviews and ratings
- Multiple address management
- Order history
- Return management
- Inventory alerts

## 📝 Notes

- SQLite database auto-creates on first run
- Images are stored in `/wwwroot/images/products/`
- Session timeout: 30 minutes
- Cart persists per user (logged in or guest)
- Products with 0 stock show as "Out of Stock"

## 🆘 Troubleshooting

**Database not creating:**
- Run `dotnet ef database update`

**Images not loading:**
- Ensure `/wwwroot/images/products/` directory exists

**Login not working:**
- Verify admin user exists in database
- Check password is correct (case-sensitive)

**Session expires:**
- Session timeout is set to 30 minutes
- Login again if session expires

## 📄 License

This project is created for educational purposes.

---

**Version:** 1.0  
**Last Updated:** August 2024  
**Status:** ✅ Production Ready
