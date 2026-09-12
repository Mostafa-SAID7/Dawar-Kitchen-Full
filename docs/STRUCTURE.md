# Project Structure

## Root

```
Naar-Noor-Full/
├── api-server/                    # ASP.NET Core 8 backend
├── naar-noor/                     # Angular 18 frontend
├── docs/                          # Documentation
├── scripts/                       # CI/CD helper scripts (Python)
├── .github/
│   └── workflows/                 # 9 GitHub Actions CI/CD pipelines
├── .agent/                        # AI agent context files
├── .husky/                        # Git hooks
├── docker-compose.yml             # Production Docker stack
├── docker-compose.dev.yml         # Development Docker stack
├── Makefile                       # Common dev commands
├── vercel.json                    # Vercel monorepo deploy config
├── dependency-check-suppression.xml # OWASP suppression rules
├── .env / .env.example            # Environment variables
├── .release-metadata.json         # Version metadata
├── VERSION                        # Current version (semver)
├── CHANGELOG.md                   # Release history
├── README.md                      # Project entry point
└── LICENSE                        # MIT License
```

---

## Backend (`api-server/`)

```
api-server/
├── NaarNoor.sln
├── Dockerfile
├── coverlet.runsettings
├── src/
│   ├── NaarNoor.API/
│   │   ├── Controllers/         # Auth, Chefs, Contact, Health,
│   │   │                        # Menu, Orders, Payments, Reports,
│   │   │                        # Reservations, Staff
│   │   ├── Configuration/       # CORS, Swagger, Health, Service configs
│   │   ├── DTOs/                # AuthDtos.cs
│   │   ├── Middleware/          # ExceptionHandling, Security, Audit,
│   │   │                        # CORS, Swagger, Controllers, Seeding
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── NaarNoor.Application/
│   │   ├── Caching/             # CacheService
│   │   ├── Chefs/               # GetChefsQuery + Handler
│   │   ├── Contact/             # SubmitInquiryCommand + Handler + Validator
│   │   ├── MenuItems/           # GetMenuItems, GetById, Create, Update, Delete
│   │   ├── Orders/              # CreateOrder, CreateStripeCheckoutSession,
│   │   │                        # HandleStripeWebhook
│   │   ├── Reservations/        # Create, Update, Delete, GetById, GetAll
│   │   ├── Services/            # IUserService
│   │   ├── Common/              # IApplicationDbContext, IRepository,
│   │   │                        # IStripeService, IUnitOfWork,
│   │   │                        # ISupabaseAuthService, ISupabaseRealtimeService,
│   │   │                        # ISupabaseStorageService, PagedResult,
│   │   │                        # ValidationBehaviour
│   │   └── DependencyInjection.cs
│   │
│   ├── NaarNoor.Domain/
│   │   ├── Entities/            # Chef, ContactInquiry, MenuItem,
│   │   │                        # Order, OrderItem, Reservation, User
│   │   ├── Enums/               # MenuCategory, OrderStatus, OrderType,
│   │   │                        # PaymentStatus, ReservationStatus
│   │   ├── ValueObjects/        # Money, TimeSlot
│   │   └── Common/BaseEntity.cs
│   │
│   └── NaarNoor.Infrastructure/
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── ApplicationDbContextFactory.cs
│       │   ├── Configurations/  # EF Core entity configs (6 files)
│       │   └── Seeds/DatabaseSeeder.cs
│       ├── Migrations/          # EF Core migrations
│       ├── Repositories/        # Repository.cs, UnitOfWork.cs
│       ├── Services/            # JwtService, StripeService,
│       │                        # SupabaseAuthService, SupabaseRealtimeService,
│       │                        # SupabaseService, SupabaseStorageService,
│       │                        # UserService
│       └── DependencyInjection.cs
│
└── tests/
    ├── NaarNoor.API.Tests/          # Integration: auth, performance, security
    ├── NaarNoor.Application.Tests/  # Chefs, Contact, MenuItems, Orders,
    │                                # Reservations, validation, DI
    ├── NaarNoor.Domain.Tests/       # Entity state validation
    └── NaarNoor.Infrastructure.Tests/ # DB, services, persistence, DI
```

**File naming:**
- Controller: `{Entity}Controller.cs`
- Command: `{Action}{Entity}Command.cs`
- Query: `Get{Entities}Query.cs`
- Handler: `{Command/Query}Handler.cs`

---

## Frontend (`naar-noor/`)

```
naar-noor/src/
├── app/
│   ├── components/          # animated-background, auth-modal, cart-drawer,
│   │   │                    # custom-calendar, custom-dropdown, footer,
│   │   │                    # header, language-toggle, toast
│   │   └── tests/           # Component spec files
│   │
│   ├── pages/               # about, checkout, contact, home, login, menu,
│   │   │                    # not-found, order-confirmed, payment-cancelled,
│   │   │                    # payment-success, privacy, register,
│   │   │                    # reservations, terms
│   │
│   ├── sections/            # about, blog, category, chefs, cinematic-banner,
│   │   │                    # hero, locations, menu, reservation
│   │
│   ├── services/            # api, auth, cart, dropdown-manager, language,
│   │   │                    # realtime, seo, theme, toast
│   │   └── tests/           # Service spec files
│   │
│   ├── models/              # auth, cart, chef, contact, menu, order,
│   │   │                    # reservation, seo, toast + index.ts
│   │
│   ├── directives/          # image-optimization, scroll-reveal
│   ├── guards/              # auth.guard.ts
│   ├── interceptors/        # auth.interceptor.ts
│   ├── app.component.ts
│   ├── app.config.ts
│   └── app.routes.ts
│
├── assets/
│   ├── i18n/                # ar.json, en.json
│   ├── icons/               # favicon.ico, favicon.svg
│   ├── blog/  chefs/  cinematic/  hero/  locations/  categories/
│   ├── manifest.json        # PWA manifest
│   ├── ngsw-config.json     # Service worker config
│   ├── robots.txt
│   ├── sitemap.xml
│   └── .htaccess
│
├── data/                    # blog.data.ts, category.data.ts,
│   │                        # chefs.data.ts, menu.data.ts
├── environments/            # environment.ts, environment.prod.ts
├── index.html
├── main.ts
├── styles.css
└── manifest.json            # PWA manifest (root copy)
```

**File naming:**
- Component: `{name}.component.ts / .html / .css`
- Service: `{name}.service.ts`
- Model: `{name}.model.ts`
- Guard: `{name}.guard.ts`
- Directive: `{name}.directive.ts`

---

## Cypress E2E (`naar-noor/cypress/`)

```
cypress/
├── e2e/                 # auth, browse-menu, cart-flow, checkout-flow,
│   │                    # menu-search, navigation, orders,
│   │                    # reservation-flow, reservation-workflow
├── fixtures/            # auth-login, chefs, menu, order-response,
│   │                    # payment-session, reservation (JSON mocks)
└── support/
    ├── commands.ts
    ├── db-isolation.ts
    ├── e2e.ts
    └── page-objects/    # LoginPage, MenuPage, OrderPage, ReservationPage
```
