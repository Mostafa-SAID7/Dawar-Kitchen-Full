# Architecture

## System Overview

```
Browser (Angular 18 SPA)
       │ HTTPS / REST + WebSocket (Supabase Realtime)
       ▼
ASP.NET Core 8 API
  ├── API Layer         (Controllers, Middleware, DTOs)
  ├── Application Layer (CQRS / MediatR / Validators)
  ├── Domain Layer      (Entities, Value Objects, Enums)
  └── Infrastructure    (EF Core, Repositories, Services)
       │ PostgreSQL (via Supabase)
       ▼
   Supabase (PostgreSQL + Auth + Storage + Realtime)

External:  Stripe (payments) · Supabase Auth (JWT)
```

---

## Backend — Clean Architecture Layers

| Layer | Project | Responsibility |
|-------|---------|---------------|
| **API** | `NaarNoor.API` | HTTP controllers, middleware, DTOs, config |
| **Application** | `NaarNoor.Application` | CQRS commands/queries, validators, cache |
| **Domain** | `NaarNoor.Domain` | Entities, enums, value objects, base entity |
| **Infrastructure** | `NaarNoor.Infrastructure` | EF Core DbContext, repositories, external services |

### CQRS Pattern

```
HTTP Request
  ├── Command (write) → Validator → Handler → EF Core → DB
  └── Query  (read)  → Cache? → Handler → EF Core → Response
```

**Data flow:** Controller → FluentValidation → MediatR Handler → EF Core → JSON

---

## Frontend — Angular 18

### Component Hierarchy

```
AppComponent
├── HeaderComponent       (nav, cart trigger, auth, language toggle)
├── Pages (routed)
│   ├── HomeComponent     (Hero, About, Menu, Chefs, Reservation, Blog sections)
│   ├── AboutComponent
│   ├── MenuComponent
│   ├── ReservationsComponent
│   ├── CheckoutComponent
│   ├── LoginComponent / RegisterComponent
│   ├── ContactComponent
│   ├── PaymentSuccess / PaymentCancelled / OrderConfirmed
│   ├── Privacy / Terms
│   └── NotFoundComponent
├── CartDrawerComponent   (slide-in cart + order form)
├── AuthModalComponent    (login/register modal)
├── ToastComponent        (notification system)
└── FooterComponent
```

### Services

| Service | Purpose |
|---------|---------|
| `ApiService` | All HTTP calls to backend (`/api/*`) |
| `AuthService` | Login, register, JWT token, Supabase Auth |
| `CartService` | Cart state + localStorage persistence |
| `RealtimeService` | Supabase Realtime subscriptions |
| `LanguageService` | EN/AR bilingual switching (i18n) |
| `ThemeService` | Dark/light mode |
| `SeoService` | Meta tags + structured data |
| `ToastService` | Toast notification queue |
| `DropdownManagerService` | Global dropdown open/close state |

---

## Tech Stack

| | Technology | Version |
|--|-----------|---------|
| **Frontend** | Angular | 18 |
| | TypeScript | 5.5 |
| | Tailwind CSS | 3.4 |
| | RxJS | 7.8 |
| **Backend** | .NET / ASP.NET Core | 8.0 |
| | Entity Framework Core | 8.0 |
| | MediatR | 12.0 |
| | FluentValidation | 11.0 |
| **Database** | PostgreSQL (Supabase) | 14+ |
| **Auth** | Supabase Auth + JWT | — |
| **Payments** | Stripe | — |
| **Docs** | Swagger / OpenAPI | 3.0 |

---

For detailed implementation see:
- [BACKEND.md](BACKEND.md) — Backend development guide
- [FRONTEND.md](FRONTEND.md) — Frontend development guide
- [DATABASE.md](DATABASE.md) — Schema and migrations
- [API.md](API.md) — REST API reference
