<div align="center">

# 🔥 Naar & Noor

**A premium full-stack restaurant platform — bilingual, real-time, and production-ready.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Version](https://img.shields.io/badge/version-1.1.1-blue)](CHANGELOG.md)
[![Tests](https://img.shields.io/badge/tests-80%25%2B%20coverage-brightgreen)](docs/TESTING.md)
[![Security](https://img.shields.io/badge/security-hardened%20%7C%200%20CVE-brightgreen)](docs/SECURITY.md)
[![Lighthouse](https://img.shields.io/badge/Lighthouse-94%2F100-orange)](docs/DEPLOYMENT.md)

[Live Demo](https://naar-noor.vercel.app) · [API Docs](docs/API.md) · [Quick Start](#-quick-start) · [Documentation](docs/README.md)

</div>

---

## ✨ Overview

**Naar & Noor** (نار و نور — Fire & Light) is a modern restaurant management platform built with Angular 18 and ASP.NET Core 8, following Clean Architecture principles. Customers can browse menus, book tables, and order online — fully in **English and Arabic**.

| Layer | Technology |
|-------|-----------|
| **Frontend** | Angular 18 · TypeScript · Tailwind CSS · RxJS |
| **Backend** | ASP.NET Core 8 · Clean Architecture · CQRS · MediatR |
| **Database** | PostgreSQL · Entity Framework Core · Supabase |
| **Payments** | Stripe (checkout sessions + webhooks) |
| **Auth** | Supabase Auth · JWT |
| **Deployment** | Docker · Vercel (frontend) · Azure App Service (backend) |
| **Testing** | Cypress E2E · Jest · xUnit · Property-based tests |
| **CI/CD** | GitHub Actions (9 workflows) |

---

## 🚀 Quick Start

```bash
# Clone
git clone https://github.com/Mostafa-SAID7/Naar-Noor-Full.git
cd Naar-Noor-Full

# Copy environment config
cp .env.example .env
# Edit .env with your Supabase + Stripe keys

# Start everything with Docker
docker-compose up -d

# App is live at:
# http://localhost        → Frontend
# http://localhost:8080   → Backend API + Swagger
```

### Local Development (without Docker)

**Backend:**
```bash
cd api-server
dotnet restore
dotnet run --project src/NaarNoor.API/NaarNoor.API.csproj
```

**Frontend:**
```bash
cd naar-noor
npm install
npm run dev          # http://localhost:4200
```

---

## 📁 Project Structure

```
Naar-Noor-Full/
├── api-server/          # ASP.NET Core 8 backend (Clean Architecture)
│   ├── src/
│   │   ├── NaarNoor.API/            # Controllers, Middleware, DTOs
│   │   ├── NaarNoor.Application/    # CQRS Commands/Queries, Validators
│   │   ├── NaarNoor.Domain/         # Entities, Value Objects, Enums
│   │   └── NaarNoor.Infrastructure/ # EF Core, Repositories, Services
│   └── tests/                       # Unit + Integration + Property tests
├── naar-noor/           # Angular 18 frontend
│   └── src/app/
│       ├── components/  # Reusable UI (header, cart, auth-modal…)
│       ├── pages/       # Routed pages (home, menu, checkout…)
│       ├── sections/    # Homepage sections (hero, chefs, blog…)
│       ├── services/    # API, auth, cart, realtime, SEO…
│       └── models/      # TypeScript interfaces
├── docs/                # Full project documentation
├── .github/workflows/   # CI/CD — 9 GitHub Actions pipelines
├── docker-compose.yml   # Production stack
├── docker-compose.dev.yml
└── Makefile             # Common dev commands
```

---

## ⚙️ CI/CD Pipelines

Nine independent, clearly-named GitHub Actions workflows:

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `secret-scan` | push / PR | Gitleaks + TruffleHog + detect-secrets |
| `unit-tests` | push / PR | Backend xUnit + Frontend Jest |
| `integration-tests` | push / PR | API integration test suite |
| `coverage-analysis` | push / PR | Coverage gates + badge generation |
| `build-artifacts` | push / PR | Docker images + Angular production build |
| `deploy` | push → main | Deploy to Vercel + Azure App Service |
| `sast-sca` | push / PR | CodeQL + OWASP dependency check |
| `lighthouse-ci` | push → main | Performance, SEO, Accessibility audits |
| `release` | tag push | Semantic versioning + GitHub Release |

---

## 🧪 Testing

```bash
# Frontend unit tests
cd naar-noor
npm test              # watch mode
npm run test:ci       # single run with coverage

# Backend tests
cd api-server
dotnet test

# E2E tests (Cypress)
cd naar-noor
npx cypress open      # interactive
npx cypress run       # headless CI mode
```

See [docs/TESTING.md](docs/TESTING.md) for full testing strategy.

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feat/my-feature`)
3. Commit your changes (pre-commit hooks run tests automatically)
4. Open a Pull Request

Please read [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) and [docs/CODE_OF_CONDUCT.md](docs/CODE_OF_CONDUCT.md) before contributing.

---

## 📚 Documentation

| Document | Description |
|----------|-------------|
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Clean Architecture, CQRS, DDD patterns |
| [docs/FRONTEND.md](docs/FRONTEND.md) | Angular structure, components, i18n |
| [docs/BACKEND.md](docs/BACKEND.md) | API design, services, auth flow |
| [docs/API.md](docs/API.md) | Endpoint reference |
| [docs/DATABASE.md](docs/DATABASE.md) | Schema, migrations, seeding |
| [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) | Docker, Vercel, Azure deployment |
| [docs/SECURITY.md](docs/SECURITY.md) | CSP, secrets, OWASP hardening |
| [docs/TESTING.md](docs/TESTING.md) | Testing strategy and coverage |
| [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) | Common issues and fixes |

---

## 📄 License

Released under the [MIT License](LICENSE). Copyright © 2026 Naar & Noor — Mostafa SAID.

---

<div align="center">

Made with ❤️ · [GitHub](https://github.com/Mostafa-SAID7/Naar-Noor-Full) · [Live Demo](https://naar-noor.vercel.app) · [Report an Issue](https://github.com/Mostafa-SAID7/Naar-Noor-Full/issues)

</div>
