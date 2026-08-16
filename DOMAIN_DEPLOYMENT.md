# Deploying NutShop to a Domain - Complete Guide

## Option 1: Azure App Service (RECOMMENDED - Easiest)

### Prerequisites
- Azure Account (free tier available)
- Visual Studio or VS Code
- Azure CLI installed

### Step 1: Create Azure Account
1. Go to https://azure.microsoft.com/free
2. Create free account (gets $200 credits)
3. Go to Azure Portal

### Step 2: Create App Service
1. Click "Create a resource"
2. Search for "App Service"
3. Click Create
4. Fill in:
   - **Resource Group:** Create new or use existing
   - **Name:** `nutshop-app` (will be: nutshop-app.azurewebsites.net)
   - **Runtime stack:** .NET 8
   - **Operating System:** Windows or Linux
   - **Region:** Choose closest to you
   - **App Service Plan:** Free F1 tier or paid
5. Click Create (takes 2-3 minutes)

### Step 3: Setup Database
1. Create Azure SQL Database or keep SQLite
2. If using SQL Database:
   - Update connection string in `appsettings.json`
   - Run migrations: `dotnet ef database update`

### Step 4: Deploy from Visual Studio
**Method A: Publish from Visual Studio**
1. Right-click project → Publish
2. Target: Azure App Service
3. Select your app service
4. Click Publish

**Method B: Publish via CLI**
```bash
# Build release
dotnet publish -c Release -o ./bin/publish

# Deploy using Azure CLI
az webapp up --name nutshop-app --resource-group YourResourceGroup
```

### Step 5: Add Custom Domain
1. In Azure Portal, go to your App Service
2. Settings → Custom domains
3. Add custom domain:
   - If using GoDaddy, Namecheap, etc.
   - Update DNS records with Azure info
   - Verify domain
4. Add SSL certificate (auto with Azure)

### Step 6: Access Your Site
- **Free domain:** https://nutshop-app.azurewebsites.net
- **Custom domain:** https://yournutshop.com

### Cost
- Free tier: $0/month (limited)
- Basic tier: ~$10-15/month
- Standard tier: $30-50/month

---

## Option 2: DigitalOcean (Best Value)

### Easiest Method: Use App Platform

1. **Create DigitalOcean Account**
   - Go to https://www.digitalocean.com
   - Sign up ($5 free credit)

2. **Connect GitHub Repository**
   - Push your code to GitHub
   - Go to DigitalOcean App Platform
   - Click Create App
   - Select GitHub repo
   - Choose branch to deploy

3. **Configure App**
   - Runtime: .NET 8
   - Build: `dotnet build -c Release`
   - Run: `dotnet run -c Release`

4. **Add Database**
   - DigitalOcean offers managed PostgreSQL
   - Update connection string

5. **Add Custom Domain**
   - In App settings
   - Add your domain
   - Update DNS records at domain registrar

### Cost
- Entry tier: $5/month
- With database: $12-20/month
- Very affordable option

---

## Option 3: AWS Elastic Beanstalk

### Step-by-Step

1. **Create AWS Account**
   - Go to https://aws.amazon.com
   - Free tier available

2. **Deploy Using EB CLI**
```bash
# Install EB CLI
pip install awsebcli

# Initialize
eb init -p "dotnet 8" nutshop

# Create environment
eb create nutshop-env

# Deploy
eb deploy
```

3. **Add Database (RDS)**
   - AWS RDS for PostgreSQL/MySQL
   - Add connection string

4. **Connect Domain**
   - Add custom domain in Route 53
   - Get SSL certificate via ACM (free)

### Cost
- Free tier: 12 months free
- After: $5-20/month depending on traffic

---

## Option 4: Traditional Web Hosting (Windows Hosting)

### Best for: GoDaddy, Bluehost, HostGator

1. **Find Windows/.NET Hosting**
   - Search "ASP.NET Core hosting"
   - Choose plan with:
     - Windows server
     - .NET 8 support
     - SQL Server or MySQL
   - Cost: $10-30/month

2. **Prepare for Upload**
```bash
# Publish to folder
dotnet publish -c Release -o ./publish
```

3. **Upload Files**
   - Use FTP client (FileZilla)
   - Upload files from publish folder
   - Update appsettings.json with database

4. **Connect Domain**
   - Point domain DNS to hosting provider
   - Usually done via control panel

5. **Set Database**
   - Create database in hosting panel
   - Update connection string
   - Run migrations

---

## Option 5: Docker + Any Cloud Provider

### For Maximum Flexibility

1. **Create Docker Image**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY bin/Release/net8.0/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "NutShop.dll"]
```

2. **Deploy to:**
   - **Heroku:** Free tier (limited)
   - **Railway.app:** $5/month
   - **Render:** $7/month
   - **AWS ECR + ECS**

3. **Build & Push Docker**
```bash
docker build -t nutshop:latest .
docker tag nutshop:latest yourregistry/nutshop:latest
docker push yourregistry/nutshop:latest
```

---

## Quick Comparison Table

| Provider | Cost | Difficulty | Setup Time | Notes |
|----------|------|-----------|-----------|--------|
| **Azure** | $0-30/mo | Easy | 10-15 min | Best for .NET |
| **DigitalOcean** | $5-20/mo | Easy | 10-15 min | Great value |
| **AWS** | $0-20/mo | Medium | 15-20 min | Powerful |
| **Heroku** | $7-50/mo | Easy | 5-10 min | Simple deployment |
| **Traditional** | $10-30/mo | Medium | 20-30 min | FTP upload |

---

## 🚀 RECOMMENDED: Azure App Service (Easiest Path)

Here's the fastest way:

### Quick Deploy to Azure (5 minutes)

1. **Install Azure CLI**
```bash
brew install azure-cli
```

2. **Login to Azure**
```bash
az login
```

3. **Create Resource Group**
```bash
az group create --name nutshop-rg --location eastus
```

4. **Create App Service Plan**
```bash
az appservice plan create --name nutshop-plan \
  --resource-group nutshop-rg --sku FREE
```

5. **Create Web App**
```bash
az webapp create --resource-group nutshop-rg \
  --plan nutshop-plan --name nutshop-app-001
```

6. **Publish Code**
```bash
dotnet publish -c Release
cd bin/Release/net8.0/publish
az webapp up --name nutshop-app-001 --resource-group nutshop-rg
```

7. **Your Site is Live!**
   - URL: `https://nutshop-app-001.azurewebsites.net`

### Add Your Own Domain

1. **Buy Domain**
   - GoDaddy, Namecheap, or any registrar
   - Cost: $10-15/year

2. **Configure in Azure**
   - Azure Portal → App Service
   - Settings → Custom domains
   - Add your domain
   - Update DNS records

3. **Get SSL Certificate**
   - Azure: Free with App Service
   - Just enable HTTPS

---

## Production Checklist

Before going live, ensure:

- [ ] Change admin password
- [ ] Set strong database password
- [ ] Enable HTTPS/SSL
- [ ] Configure email notifications
- [ ] Setup database backups
- [ ] Monitor application logs
- [ ] Setup error tracking
- [ ] Add Google Analytics (optional)
- [ ] Enable CDN for images (optional)
- [ ] Setup auto-scaling (optional)

---

## Database Connection Update for Production

Update `appsettings.json` for production:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=nutshop;User Id=sa;Password=YourPassword;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

Or use environment variables:
```bash
export ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"
```

---

## Cost Estimation

### Option 1: Azure (Most Recommended)
- **Free to Start:** $0 (free tier for 12 months)
- **Small Business:** $10-15/month
- **Growing Business:** $30-100/month

### Option 2: DigitalOcean
- **Starter:** $5/month (CPU only)
- **With Database:** $12-20/month
- **Scalable:** $20-100+/month

### Option 3: Traditional Hosting
- **Budget:** $10-15/month
- **Standard:** $20-30/month
- **Premium:** $50+/month

---

## Domain Registrars (Cheapest Options)

| Registrar | Domain Cost | Hosting | Both Together |
|-----------|------------|---------|---------------|
| **Namecheap** | $8.88/yr | $4/mo | Cheap |
| **GoDaddy** | $9.99/yr | $8/mo | Expensive |
| **Google Domains** | $12/yr | - | Google integrated |
| **Porkbun** | $6/yr | - | Cheapest |

---

## My Recommendation for You

**Best Option: Azure App Service**
- ✅ Free to start ($0 setup)
- ✅ Easy for .NET Core apps
- ✅ 1-click custom domain
- ✅ Automatic SSL certificates
- ✅ Excellent support
- ✅ Scales automatically
- ✅ Pay only for what you use

**Total Monthly Cost:**
- App Service: $10-15
- Domain: $0.80 (split monthly)
- **Total: ~$11/month**

---

## Summary

1. **Choose Provider** (Azure recommended)
2. **Buy Domain** (Namecheap or GoDaddy)
3. **Deploy Code** (CLI or VS publish)
4. **Update Database** (SQL Server or PostgreSQL)
5. **Add Domain** (DNS configuration)
6. **Go Live!**

Your NutShop will be available at `yournutshop.com` within 1-2 hours!
