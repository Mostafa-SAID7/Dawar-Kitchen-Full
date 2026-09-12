# Dawar Kitchen Deployment Guide

## Project Architecture

```
Naar-Noor (Full Stack)
├── Backend API (ASP.NET Core 8.0) → Docker
├── Frontend Web (Angular 18) → Docker  
├── Mobile App (Expo/React Native) → Native apps + Web
└── Desktop App (WinForms .NET) → Windows Installer
```

---

## 1. Development Setup (Docker Compose)

### Prerequisites
- Docker & Docker Compose installed
- PostgreSQL 16 (via Docker)
- Node.js 20+ (for frontend)
- .NET 8.0 SDK (for backend)

### Start Development Environment

```bash
cd Naar-Noor

# Set environment variables
export PGPASSWORD=dev_password_change_me
export SUPABASE_URL=http://localhost:54321
export SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZS1kZW1vIiwicm9sZSI6ImFub24ifQ.625_WdcF3KHqz5amU0x2KfUqKS4FTO2p9PsrqjIjxQA
export SUPABASE_SERVICE_ROLE_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZS1kZW1vIiwicm9sZSI6InNlcnZpY2Vfcm9sZSJ9.DaYlG6CDqKs0V8Z-LTUItzjhck01OMc3IrapzP-kiGI

# Start all services (frontend, backend, database)
docker-compose -f docker-compose.dev.yml up

# Or start services in background
docker-compose -f docker-compose.dev.yml up -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f

# Stop services
docker-compose -f docker-compose.dev.yml down
```

### Service URLs

| Service | URL | Port |
|---------|-----|------|
| Frontend (Angular) | http://localhost:4200 | 4200 |
| Backend API | http://localhost:8080 | 8080 |
| Database Adminer | http://localhost:8081 | 8081 |
| PostgreSQL | localhost:5432 | 5432 |

### Database Access (Adminer)

1. Go to http://localhost:8081
2. Select **PostgreSQL** from dropdown
3. **Server**: `naar-noor-dev-database`
4. **Username**: `postgres`
5. **Password**: `dev_password_change_me` (or your PGPASSWORD)
6. **Database**: `postgres`
7. Click **Login**

---

## 2. Mobile App Deployment

### Development (Local Testing)

```bash
cd mobile

# Install dependencies
npm install

# Start Expo server
npm start

# Options in terminal:
# - Press 'i' for iOS simulator
# - Press 'a' for Android emulator
# - Press 'w' for web browser
# - Scan QR code with Expo Go app on physical device
```

### Production Release

#### Option A: Expo EAS (Recommended)

```bash
# Install EAS CLI
npm install -g eas-cli

# Login to Expo
eas login

# Configure EAS (if not already done)
eas build:configure

# Build for iOS
eas build --platform ios --auto-submit

# Build for Android
eas build --platform android --auto-submit

# Submit to app stores
eas submit -p ios
eas submit -p android
```

#### Option B: Manual Build

**iOS:**
```bash
eas build -p ios
# Then upload to App Store Connect
```

**Android:**
```bash
eas build -p android
# Then upload to Google Play Console
```

### Testing on Device

1. Install **Expo Go** app from App Store/Play Store
2. Run `npm start` in mobile directory
3. Scan QR code with Expo Go
4. App loads in minutes

---

## 3. Desktop App Deployment

### Development (Local Testing)

```bash
cd desktop

# Install dependencies
dotnet restore

# Run application
dotnet run --project src/NaarNoor.Desktop.WinForms

# Or with hot reload
dotnet watch run --project src/NaarNoor.Desktop.WinForms
```

### Production Release (Windows Installer)

#### Prerequisites
- Visual Studio 2022 or Build Tools
- WiX Toolset (for MSI installer) - Optional

#### Step 1: Publish Application

```bash
cd api-server
dotnet publish -c Release -o ./publish
```

#### Step 2: Create Installer

```bash
# Option A: Self-contained executable (recommended)
dotnet publish -c Release -r win-x64 --self-contained -o ./publish

# Creates: NaarNoor.Desktop.exe (can run without .NET installed)
```

#### Step 3: Package for Distribution

```bash
# Create ZIP archive
cd publish
Compress-Archive -Path NaarNoor.Desktop.exe -DestinationPath NaarNoor-Desktop-v1.0.0.zip

# Or create MSI installer (if using WiX)
# See: heat.exe, candle.exe, light.exe in WiX documentation
```

#### Step 4: Distribution

- Host on GitHub Releases
- Host on company website
- Distribute via Windows Update (if applicable)

**Update Check:**
Add auto-update mechanism in Program.cs
```csharp
// Check for updates at startup
var latestVersion = await UpdateChecker.GetLatestVersionAsync();
if (latestVersion > CurrentVersion)
{
    if (MessageBox.Show("Update available. Download now?", "Update", MessageBoxButtons.YesNo) == DialogResult.Yes)
    {
        Process.Start("https://github.com/Mostafa-SAID7/Naar-Noor/releases");
    }
}
```

---

## 4. Backend API Deployment

### Docker Production Build

```bash
# Build image
docker build -f api-server/Dockerfile -t naar-noor:backend:latest .

# Push to registry
docker tag naar-noor:backend:latest myregistry.azurecr.io/naar-noor:backend:latest
docker push myregistry.azurecr.io/naar-noor:backend:latest

# Run container
docker run -d \
  -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e POSTGRESQL_CONNECTION_STRING="Host=db.example.com;Database=postgres;User Id=postgres;Password=***" \
  -e Supabase__Url="https://your-project.supabase.co" \
  -e Supabase__AnonKey="your-anon-key" \
  -e Supabase__ServiceRoleKey="your-service-role-key" \
  naar-noor:backend:latest
```

### Azure Container Registry Deployment

```bash
# Create ACR
az acr create --resource-group myResourceGroup --name naar-noor --sku Basic

# Login
az acr login --name naar-noor

# Tag & push
docker tag naar-noor:backend naar-noor.azurecr.io/backend:latest
docker push naar-noor.azurecr.io/backend:latest

# Deploy to Azure Container Instances
az container create \
  --resource-group myResourceGroup \
  --name naar-noor-api \
  --image naar-noor.azurecr.io/backend:latest \
  --registry-login-server naar-noor.azurecr.io \
  --registry-username <username> \
  --registry-password <password> \
  --environment-variables ASPNETCORE_ENVIRONMENT=Production
```

### Kubernetes Deployment

See `k8s/` directory for Kubernetes manifests:

```bash
# Deploy to cluster
kubectl apply -f k8s/

# Check status
kubectl get pods
kubectl logs -f deployment/naar-noor-api
```

---

## 5. Frontend Web Deployment

### Docker Production Build

```bash
# Build image
docker build -f naar-noor/Dockerfile -t naar-noor:frontend:latest .

# Run container
docker run -d -p 80:80 naar-noor:frontend:latest
```

### Vercel Deployment (Recommended for Angular)

```bash
# Install Vercel CLI
npm install -g vercel

# Deploy
vercel --prod

# Or use GitHub integration:
# 1. Push to GitHub
# 2. Connect repo in Vercel dashboard
# 3. Automatic deploys on push
```

### Static Hosting (AWS S3 + CloudFront)

```bash
# Build production bundle
npm run build

# Upload to S3
aws s3 sync dist/ s3://naar-noor-frontend/

# Invalidate CloudFront cache
aws cloudfront create-invalidation --distribution-id E123 --paths "/*"
```

---

## 6. Environment Configuration

### Development (.env.example)

```env
# API
Api__BaseUrl=http://localhost:8080
Api__TimeoutSeconds=30

# Database
PGPASSWORD=dev_password_change_me

# Supabase (Development/Mock)
SUPABASE_URL=http://localhost:54321
SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZS1kZW1vIiwicm9sZSI6ImFub24ifQ.625_WdcF3KHqz5amU0x2KfUqKS4FTO2p9PsrqjIjxQA
SUPABASE_SERVICE_ROLE_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZS1kZW1vIiwicm9sZSI6InNlcnZpY2Vfcm9sZSJ9.DaYlG6CDqKs0V8Z-LTUItzjhck01OMc3IrapzP-kiGI

# Mobile
EXPO_PUBLIC_API_URL=http://localhost:8080
```

### Production (.env.production - SECURE)

```env
# API
Api__BaseUrl=https://api.naar-noor.com
Api__TimeoutSeconds=30

# Database (Use Azure Key Vault / AWS Secrets Manager)
POSTGRESQL_CONNECTION_STRING=***SECRET***

# Supabase (Production)
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_ANON_KEY=***SECRET***
SUPABASE_SERVICE_ROLE_KEY=***SECRET***

# Mobile
EXPO_PUBLIC_API_URL=https://api.naar-noor.com
```

---

## 7. Health Checks & Monitoring

### API Health Check

```bash
# Local development
curl http://localhost:8080/health

# Production
curl https://api.naar-noor.com/health
```

### Docker Health Status

```bash
# Check container health
docker ps --format "table {{.Names}}\t{{.Status}}"

# View health logs
docker inspect naar-noor-prod-api | grep -A 5 '"Health"'
```

### Monitoring Stack (Optional)

```yaml
# Add to docker-compose for production monitoring
prometheus:
  image: prom/prometheus:latest
  ports:
    - "9090:9090"
  volumes:
    - ./prometheus.yml:/etc/prometheus/prometheus.yml

grafana:
  image: grafana/grafana:latest
  ports:
    - "3000:3000"
  depends_on:
    - prometheus
```

---

## 8. Troubleshooting

### Backend API not starting

```bash
# Check database connection
docker-compose logs naar-noor-dev-api

# Verify PostgreSQL is running
docker-compose logs naar-noor-dev-database

# Restart services
docker-compose down && docker-compose up
```

### Frontend showing blank page

```bash
# Clear browser cache
# Check API URL in environment
echo $API_URL

# Rebuild Docker image
docker-compose build --no-cache naar-noor-dev-web
```

### Mobile app can't connect to API

```bash
# Check API URL in mobile/.env
cat mobile/.env

# Test API from mobile device
curl http://your-machine-ip:8080/health

# On Windows, get your IP
ipconfig
```

### Database migration issues

```bash
# Connect to database and check schema
docker-compose exec naar-noor-dev-database psql -U postgres -d postgres

# Or use Adminer UI
open http://localhost:8081
```

---

## 9. Checklist for Production Release

- [ ] All tests passing (desktop, mobile, API)
- [ ] Environment variables configured securely
- [ ] Database backups configured
- [ ] Health checks operational
- [ ] Monitoring/logging setup
- [ ] SSL certificates configured (HTTPS)
- [ ] CORS policies reviewed
- [ ] Authentication/authorization tested
- [ ] Rate limiting configured
- [ ] Documentation updated
- [ ] Release notes prepared
- [ ] Rollback plan documented

---

## Support

For issues or questions:
- GitHub Issues: https://github.com/Mostafa-SAID7/Naar-Noor/issues
- Documentation: See `/docs` folder
- API Docs: Swagger UI at `/swagger` endpoint
