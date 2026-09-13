# Base44 Dev Environment — Naar & Noor (Dawar Kitchen)

## Overview
Full-stack restaurant platform: Angular 18 frontend (`dawar-kitchen/`) + ASP.NET Core 8 backend (`api-server/`) + PostgreSQL.

## Architecture
- **Frontend**: Angular 18, dev server on port 5000 (mapped to host 3000). Uses `@angular/build:dev-server` builder with `allowedHosts: true`. API calls go to `/api/*` via Angular dev-server proxy.
- **Backend**: ASP.NET Core 8 Clean Architecture. Binds to `PORT` env var (set to 8080) via `WebHostConfiguration.cs` — `ASPNETCORE_URLS` is NOT used. Auto-migrates and seeds the database on startup (`DatabaseSeeder.SeedAsync`).
- **Database**: PostgreSQL 16 in compose. Connection via `PGHOST/PGUSER/PGPASSWORD/PGDATABASE` env vars (checked first by `BuildConnectionString` in `Infrastructure/DependencyInjection.cs`).

## Key Details
- The frontend has **mock data fallbacks** — it renders even if the backend is down (API service catches errors and returns mock menu/chefs).
- The proxy config `dawar-kitchen/proxy.conf.base44.json` targets `http://api:8080` (the backend container hostname). The repo's own `proxy.conf.json` targets `http://localhost:8080` for local dev.
- JWT secret key has a default in `appsettings.json` (49 chars, min 32 required) — no env var needed.
- Supabase and Stripe are **optional** — empty strings are handled gracefully. No external secrets required to boot.
- CORS: Development mode allows any origin (no `CORS_ALLOWED_ORIGINS` needed).

## Running
```bash
docker compose -f docker-compose.base44.yml up -d --build
```
- Frontend: http://localhost:3000 (preview)
- Backend API + Swagger: http://localhost:8000
- Backend health: http://localhost:8000/health

## Optional External Integrations
If the user wants real Supabase auth/storage or Stripe payments, they need to provide:
- `SUPABASE_URL`, `SUPABASE_ANON_KEY`, `SUPABASE_SERVICE_ROLE_KEY`
- `STRIPE_SECRET_KEY`, `STRIPE_PUBLISHABLE_KEY`, `STRIPE_WEBHOOK_SECRET`
These are NOT required for the app to boot or render.
