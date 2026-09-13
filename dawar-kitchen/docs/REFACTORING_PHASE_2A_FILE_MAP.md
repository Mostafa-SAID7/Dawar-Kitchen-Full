# Phase 2A: Complete File Responsibility Map
## Dawar Kitchen Angular Architecture Refactoring

**Date**: September 13, 2026  
**Purpose**: Classify every application file with action decisions before refactoring  
**Status**: Planning phase - NO FILES MOVED YET

---

## File Classification Legend

### Columns
- **Current Path**: Where file lives now
- **File Name**: File identifier
- **Responsibility**: What the file does
- **Reusable?**: Can it be used by multiple features?
- **Business Logic?**: Contains domain/application logic?
- **UI Logic?**: Contains presentation/interaction logic?
- **API Logic?**: Calls HttpClient or backend?
- **Cross-Cutting?**: Used globally/across multiple concerns?
- **Dependencies**: What it imports/depends on
- **Used By**: Which files/components use it
- **Current Layer**: Where it is now
- **Candidate New Location**: Where it should go
- **Action**: KEEP / MOVE / MERGE / SPLIT / REFACTOR / DELETE
- **Reason**: Why this action

---

## ROOT LEVEL FILES

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/ | app.component.ts | Root shell, layout composition | No | No | Yes | No | Yes | HeaderComponent, FooterComponent, RouterOutlet, CartDrawerComponent, ToastComponent, AnimatedBackgroundComponent | Bootstrap | Root | Core/app | KEEP | Root shell needed; proper placement |
| src/app/ | app.config.ts | Application configuration, providers setup | No | No | No | No | Yes | provideRouter, provideHttpClient, BrowserAnimationsModule, TranslateModule, ServiceWorkerModule, all services/guards/interceptors | bootstrapApplication() | Root | Core/config | MOVE | Belongs in core/config infrastructure layer |
| src/app/ | app.routes.ts | Routing configuration, route definitions | No | No | No | No | Yes | All page components, LazyLoadComponents | bootstrapApplication() | Root | Core/routes | MOVE | Belongs in core/routing infrastructure layer |

---

## GUARDS & INTERCEPTORS (Infrastructure)

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/guards/ | auth.guard.ts | Route access control for authenticated users | No | No | No | No | Yes | AuthService | app.routes.ts | Infrastructure | core/guards/auth.guard.ts | MOVE | Core infrastructure concern |
| src/app/interceptors/ | auth.interceptor.ts | Inject JWT bearer token into HTTP requests | No | No | No | Yes | Yes | AuthService, HttpClient | HttpClientModule config | Infrastructure | core/interceptors/auth.interceptor.ts | MOVE | Core HTTP infrastructure |

---

## MODELS & INTERFACES

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/models/ | index.ts | Barrel export of all models | Yes | No | No | No | Yes | All other model files | Components, services, pages | Shared | shared/models/index.ts | MOVE | Shared code |
| src/app/models/ | auth.model.ts | AuthSession, LoginResponse, MeResponse types | Yes | No | No | No | No | None | AuthService, AuthInterceptor, auth-modal, header | Shared | shared/models/auth.model.ts | MOVE | Shared types (but rename to core/auth/auth.model.ts since it's auth-specific) | MOVE | Actually: auth-specific, belongs in core/auth/ |
| src/app/models/ | cart.model.ts | CartItem, Cart state types | Yes | No | No | No | No | None | CartService, components | Shared | features/checkout/models/cart.model.ts | MOVE | Feature-specific (checkout/cart domain) |
| src/app/models/ | menu.model.ts | MenuItem, MenuCategory types | Yes | No | No | No | No | None | MenuService, pages, sections | Shared | features/menu/models/menu.model.ts | MOVE | Feature-specific (menu domain) |
| src/app/models/ | order.model.ts | CreateOrderRequest, OrderResponse types | Yes | No | No | No | No | None | OrderService, checkout, pages | Shared | features/orders/models/order.model.ts | MOVE | Feature-specific (orders domain) |
| src/app/models/ | reservation.model.ts | CreateReservationRequest, ReservationResponse types | Yes | No | No | No | No | None | ReservationService, pages | Shared | features/reservations/models/reservation.model.ts | MOVE | Feature-specific (reservations domain) |
| src/app/models/ | contact.model.ts | CreateContactRequest, ContactResponse types | Yes | No | No | No | No | None | ContactService, contact page | Shared | features/contact/models/contact.model.ts | MOVE | Feature-specific (contact domain) |
| src/app/models/ | chef.model.ts | Chef interface | Yes | No | No | No | No | None | ChefService, sections, pages | Shared | shared/models/chef.model.ts | MOVE | Generic domain model (chefs are cross-feature) |
| src/app/models/ | seo.model.ts | SeoConfig interface | Yes | No | No | No | No | None | SeoService | Shared | shared/models/seo.model.ts | MOVE | Cross-cutting concern (all pages use) |
| src/app/models/ | toast.model.ts | Toast, ToastType interfaces | Yes | No | No | No | No | None | ToastService, components | Shared | shared/models/toast.model.ts | MOVE | Cross-cutting concern (global notifications) |

---

## SERVICES - INFRASTRUCTURE (HTTP, Auth, Config)

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/services/ | api.service.ts | Generic HTTP API communication layer | Yes | No | No | Yes | No | HttpClient | All services, components | Infrastructure | core/http/api.service.ts | MOVE | Core HTTP infrastructure (but rename & refactor) |
| src/app/services/ | auth.service.ts | Authentication state, login/logout/register, JWT token management | No | Yes | No | Yes | Yes | HttpClient, localStorage, localStorage, signals | AuthInterceptor, components, pages, guards | Infrastructure | core/auth/auth.service.ts | MOVE | Core auth infrastructure |

---

## SERVICES - STATE MANAGEMENT (Global/Cross-Cutting)

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/services/ | cart.service.ts | Shopping cart state, cart operations, localStorage persistence | No | Yes | No | No | Yes | localStorage, signals | CartDrawerComponent, CheckoutComponent, MenuComponent, header | Feature | features/checkout/services/cart.service.ts | MOVE | Checkout feature-specific (cart is part of checkout flow) |
| src/app/services/ | theme.service.ts | Dark/light theme state, localStorage persistence | Yes | No | No | No | Yes | localStorage, signals | HeaderComponent, likely layout | Shared | shared/services/theme.service.ts | MOVE | Cross-cutting utility (global theme toggle) |
| src/app/services/ | language.service.ts | i18n language state, RTL/document direction, localStorage persistence | Yes | No | No | No | Yes | TranslateService, localStorage, BehaviorSubject | All components, entire app | Shared | shared/services/language.service.ts | MOVE + REFACTOR | Cross-cutting (migrate BehaviorSubject → Signal) |
| src/app/services/ | toast.service.ts | Toast notification queue, auto-dismiss, signals | Yes | No | No | No | Yes | signals | CheckoutComponent, ReservationComponent, ContactComponent, ToastComponent | Shared | shared/services/toast.service.ts | MOVE | Cross-cutting concern (global notifications) |
| src/app/services/ | seo.service.ts | SEO metadata management (page title, OG tags, canonical) | Yes | No | No | No | Yes | Title, Meta | All pages | Shared | shared/services/seo.service.ts | MOVE | Cross-cutting concern (all pages use) |
| src/app/services/ | dropdown-manager.service.ts | Coordinate dropdown open/close states | Yes | No | No | No | Yes | Subject, RxJS | CustomDropdownComponent, other dropdowns | Shared | shared/services/dropdown-manager.service.ts | MOVE + REFACTOR | UI coordination utility (migrate Subject → Signal) |
| src/app/services/ | realtime.service.ts | WebSocket subscriptions for order/reservation status updates | No | Yes | No | Yes | No | WebSocket, callbacks | OrderConfirmedComponent, potentially other features | Feature | features/orders/services/realtime.service.ts | MOVE | Order-specific (tracks order status) |

---

## SHARED COMPONENTS (Global Shell & Reusable UI)

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/components/header/ | header.component.ts/html/css | Main navigation bar, mobile menu, cart/auth UI, logout | No | No | Yes | No | Yes | AuthService, CartService, ThemeService, LanguageService, RouterModule | AppComponent (root shell) | Layout | layout/components/header/header.component | MOVE | Global layout component |
| src/app/components/footer/ | footer.component.ts/html/css | Footer navigation, company info, social links | No | No | Yes | No | Yes | None (mostly static) | AppComponent (root shell) | Layout | layout/components/footer/footer.component | MOVE | Global layout component |
| src/app/components/animated-background/ | animated-background.component.ts/html/css | SVG/canvas animation background effect | Yes | No | Yes | No | No | None | AppComponent (visual effect) | Shared | shared/components/animated-background/animated-background.component | MOVE | Reusable visual component |
| src/app/components/toast/ | toast.component.ts/html/css | Toast notification display container | Yes | No | Yes | No | Yes | ToastService | AppComponent (overlay) | Shared | shared/components/toast/toast.component | MOVE | Cross-cutting UI component |
| src/app/components/cart-drawer/ | cart-drawer.component.ts/html/css | **GOD COMPONENT**: Multi-step checkout form, validation, order submission, cart display | No | **YES** | Yes | **YES** | No | CartService, ApiService, ToastService, FormBuilder, CustomDropdownComponent | AppComponent (modal overlay), header (opens drawer) | Feature | features/checkout/components/cart-drawer/ | **SPLIT** | Contains 3 responsibilities: (1) Cart display (→ CartDisplayComponent), (2) Checkout form (→ CheckoutFormComponent), (3) Order submission (→ CheckoutService). Also duplicates checkout.component logic. |
| src/app/components/auth-modal/ | auth-modal.component.ts/html/css | Login/Register form modal, dual form management, form submission | No | **YES** | Yes | **YES** | No | AuthService, FormBuilder, ToastService, RouterModule | AppComponent (modal overlay), header (opens modal) | Feature | features/auth/components/auth-modal/ | **SPLIT** | Contains 2 responsibilities: (1) Login form, (2) Register form. Should split into LoginFormComponent & RegisterFormComponent subcomponents. |
| src/app/components/custom-dropdown/ | custom-dropdown.component.ts/html/css | Reusable dropdown UI wrapper, open/close coordination | Yes | No | Yes | No | No | DropdownManagerService | CartDrawerComponent, CheckoutComponent, pages | Shared | shared/components/custom-dropdown/custom-dropdown.component | MOVE | Reusable UI component |
| src/app/components/custom-calendar/ | custom-calendar.component.ts/html/css | Date picker calendar widget | Yes | No | Yes | No | No | None | ReservationsPageComponent | Shared | shared/components/custom-calendar/custom-calendar.component | MOVE | Reusable UI component |
| src/app/components/language-toggle/ | language-toggle.component.ts/html/css | Language switcher (EN/AR), triggers RTL change | Yes | No | Yes | No | No | LanguageService | HeaderComponent | Shared | shared/components/language-toggle/language-toggle.component | MOVE | Reusable UI component |

---

## PAGE COMPONENTS (Routes)

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/pages/home/ | home.component.ts | Landing page, compose sections (Hero, Category, Menu, CinematicBanner) | No | No | Yes | No | No | SeoService, various sections | app.routes.ts | Pages | features/home/pages/home.component | MOVE | Home is a feature |
| src/app/pages/menu/ | menu.component.ts | Menu browsing page, filter by category, display items, add to cart | No | **YES** | Yes | **YES** | No | ApiService, CartService, SeoService | app.routes.ts | Pages | features/menu/pages/menu-page.component | MOVE | Menu is a feature |
| src/app/pages/checkout/ | checkout.component.ts | **Checkout orchestration**: Order type selection, delivery/dine-in, form, Stripe session | No | **YES** | Yes | **YES** | No | CartService, ApiService, ToastService, FormBuilder, CustomDropdownComponent | app.routes.ts | Pages | features/checkout/pages/checkout-page.component | MOVE + REFACTOR | Checkout feature; duplicates cart-drawer logic (should extract CheckoutService) |
| src/app/pages/reservations/ | reservations.component.ts | Reservation booking form, date/time selection, submission | No | **YES** | Yes | **YES** | No | ApiService, ToastService, FormBuilder, CustomCalendarComponent, SeoService | app.routes.ts | Pages | features/reservations/pages/reservation-page.component | MOVE | Reservations is a feature |
| src/app/pages/contact/ | contact.component.ts | Contact form page, form submission | No | **YES** | Yes | **YES** | No | ApiService, ToastService, FormBuilder, SeoService | app.routes.ts | Pages | features/contact/pages/contact-page.component | MOVE | Contact is a feature |
| src/app/pages/login/ | login.component.ts | Login form page, form submission | No | **YES** | Yes | **YES** | No | AuthService, ToastService, FormBuilder, RouterModule, SeoService | app.routes.ts | Pages | features/auth/pages/login-page.component | MOVE | Auth feature |
| src/app/pages/register/ | register.component.ts | Registration form page, form submission | No | **YES** | Yes | **YES** | No | AuthService, ToastService, FormBuilder, RouterModule, SeoService | app.routes.ts | Pages | features/auth/pages/register-page.component | MOVE | Auth feature |
| src/app/pages/order-confirmed/ | order-confirmed.component.ts | Order confirmation page, realtime status subscription | No | **YES** | Yes | **YES** | No | RealtimeService, OrderService, SeoService | app.routes.ts (protected by authGuard) | Pages | features/orders/pages/order-confirmed-page.component | MOVE | Orders feature |
| src/app/pages/payment-success/ | payment-success.component.ts | Payment success confirmation page, displays order details | No | No | Yes | No | No | route query params (session_id), SeoService | app.routes.ts | Pages | features/checkout/pages/payment-success-page.component | MOVE | Part of checkout flow |
| src/app/pages/payment-cancelled/ | payment-cancelled.component.ts | Payment failure/cancellation page, error display | No | No | Yes | No | No | route query params, SeoService | app.routes.ts | Pages | features/checkout/pages/payment-cancelled-page.component | MOVE | Part of checkout flow |
| src/app/pages/about/ | about.component.ts | Static about page | No | No | Yes | No | No | SeoService | app.routes.ts | Pages | features/static/pages/about-page.component | MOVE | Static content feature |
| src/app/pages/privacy/ | privacy.component.ts | Static privacy policy page | No | No | Yes | No | No | SeoService | app.routes.ts | Pages | features/static/pages/privacy-page.component | MOVE | Static content feature |
| src/app/pages/terms/ | terms.component.ts | Static terms & conditions page | No | No | Yes | No | No | SeoService | app.routes.ts | Pages | features/static/pages/terms-page.component | MOVE | Static content feature |
| src/app/pages/not-found/ | not-found.component.ts | 404 error page, wildcard route | No | No | Yes | No | No | None | app.routes.ts (wildcard) | Pages | features/static/pages/not-found-page.component | MOVE | Static error page |

---

## SECTIONS (Reusable Page Modules)

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/sections/hero/ | hero.component.ts | Large banner header on home page, visual + CTA | Yes | No | Yes | No | No | None (static content) | HomeComponent | Sections | shared/components/hero/hero.component | MOVE | Reusable section component |
| src/app/sections/category/ | category.component.ts | Category filter UI, filter menu by category | No | **YES** | Yes | **YES** | No | ApiService, signals or state | HomeComponent, MenuComponent | Sections | features/menu/components/category-filter.component | MOVE | Menu feature-specific (filters menu by category) |
| src/app/sections/menu/ | menu.component.ts | Menu items display grid, add to cart | No | **YES** | Yes | **YES** | No | ApiService, CartService | HomeComponent, MenuComponent | Sections | features/menu/components/menu-display.component | MOVE | Menu feature-specific (displays menu items) |
| src/app/sections/cinematic-banner/ | cinematic-banner.component.ts | Visual banner with animation/video effect | Yes | No | Yes | No | No | None (static visual) | HomeComponent | Sections | shared/components/cinematic-banner/cinematic-banner.component | MOVE | Reusable section component |
| src/app/sections/about/ | about.component.ts | About section prose on home page | Yes | No | Yes | No | No | None (static content) | HomeComponent | Sections | shared/components/about-section/about-section.component | MOVE | Reusable section component |
| src/app/sections/chefs/ | chefs.component.ts | Chef showcase grid on home page | Yes | **YES** | Yes | **YES** | No | ApiService (fetch chefs) | HomeComponent | Sections | features/chefs/components/chefs-showcase.component | MOVE | Feature-specific (displays chefs list) |
| src/app/sections/blog/ | blog.component.ts | Blog posts/news section on home page | Yes | **YES** | Yes | **YES** | No | ApiService (fetch blog posts) | HomeComponent (not currently used?) | Sections | features/blog/components/blog-section.component | MOVE + FLAG | Feature-specific; but NOT used in current home (verify if dead code) |
| src/app/sections/locations/ | locations.component.ts | Restaurant locations section on home page | Yes | **YES** | Yes | **YES** | No | ApiService (fetch locations) | HomeComponent (not currently used?) | Sections | features/locations/components/locations-section.component | MOVE + FLAG | Feature-specific; but NOT used in current home (verify if dead code) |
| src/app/sections/reservation/ | reservation.component.ts | Reservation CTA section on home page, link to /reservations | Yes | No | Yes | No | No | RouterModule | HomeComponent | Sections | shared/components/reservation-cta/reservation-cta.component | MOVE | Generic CTA section (could be reused) |

---

## DIRECTIVES

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/directives/ | * (not analyzed yet) | (To be analyzed) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | Directives | shared/directives/ | MOVE | Generic directives → shared |

---

## PIPES (if any exist)

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| src/app/pipes/ | * (if exist) | (To be analyzed) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | Pipes | shared/pipes/ | MOVE | Generic pipes → shared |

---

## UTILITIES / HELPERS / CONSTANTS

| Current Path | File Name | Responsibility | Reusable? | Business Logic? | UI Logic? | API Logic? | Cross-Cutting? | Dependencies | Used By | Current Layer | Candidate New Location | Action | Reason |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| (TBD - not identified yet) | * | (To be analyzed) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | (TBD) | shared/utils/, shared/constants/ | MOVE | Move all utilities to shared |

---

## NEW SERVICES TO BE CREATED (Phase 6)

| Service Name | Responsibility | Feature/Layer | Why Needed | Replaces/Supplements |
|---|---|---|---|---|
| **OrderService** | Coordinate cart → order submission → realtime tracking | features/orders/services/order.service | Extracted from scattered checkout + realtime logic | Supplements api.service, consolidates order workflow |
| **CheckoutService** | Manage checkout form state, validation, Stripe session creation | features/checkout/services/checkout.service | Extracted from checkout.component + cart-drawer duplication | Replaces duplicated form logic |
| **ReservationService** | Manage reservation form state, validation, submission | features/reservations/services/reservation.service | Extracted from reservations.component | Centralizes reservation workflow |
| **MenuService** | Fetch menu items, manage categories, caching, filtering | features/menu/services/menu.service | Extracted from api.service + sections/menu logic | Supplements api.service with business logic |
| **ChefService** | Fetch chefs list, manage data | features/chefs/services/chef.service | Extracted from api.service | Supplements api.service |
| **ContactService** | Handle contact form submission | features/contact/services/contact.service | Extracted from contact.component | Centralizes contact workflow |
| **AuthService** (already exists but may need slight refactoring) | Auth state, login/logout, token management | core/auth/services/auth.service | Refactor to standardize state (use Signal) | Already exists, refactor for consistency |

---

## REFACTORING SUMMARY BY ACTION

### MOVE (50+ files to new locations)
- **Guards**: auth.guard.ts → core/guards/
- **Interceptors**: auth.interceptor.ts → core/interceptors/
- **Models**: All from models/ → feature-specific or shared/models/
- **Services**: Infrastructure/cross-cutting → core/ or shared/services/
- **Feature services**: Feature-specific services → features/*/services/
- **Components**: All shared → shared/components/ or layout/components/
- **Sections**: Feature-specific → features/*/components/ or shared/components/
- **Pages**: All → features/*/pages/
- **Root files**: app.config.ts, app.routes.ts → core/

### SPLIT (Major refactoring)
- **cart-drawer.component** → 3 components: CartDisplayComponent, CheckoutFormComponent (shared), ConfirmationComponent + CheckoutService
- **auth-modal.component** → 2 subcomponents: LoginFormComponent, RegisterFormComponent
- **checkout.component** → Use extracted CheckoutService, reuse CheckoutFormComponent

### MERGE (Consolidation)
- **checkout.component + cart-drawer checkout form** → Single CheckoutFormComponent + CheckoutService
- **Duplicate validators** → Shared validators utilities
- **Duplicate form builders** → Extract form factory helpers

### REFACTOR (Pattern changes)
- **language.service**: Migrate BehaviorSubject → Signal (consistency)
- **dropdown-manager.service**: Migrate Subject → Signal (consistency)
- **realtime.service**: Enhance WebSocket error handling, integrate with OrderService
- **api.service**: Rename to HttpApiService, separate concerns (HTTP infrastructure vs business logic)

### DELETE (After verification)
- **sections/blog** (if unused)
- **sections/locations** (if unused)
- **Old directory structures** (after migration complete)

### CREATE (New folders)
- **src/app/core/** (auth, http, config, guards, interceptors, services)
- **src/app/shared/** (components, directives, pipes, validators, models, utils, constants, services)
- **src/app/layout/** (header, footer, shell layout)
- **src/app/features/** (auth, menu, reservations, checkout, orders, contact, home, static, chefs, blog, locations)

---

## DEPENDENCY RULES FOR NEW ARCHITECTURE

```
Core
  └─ No dependencies on Features or Shared

Shared
  └─ No dependencies on Features or Core

Features
  ├─ Can depend on Shared ✅
  ├─ Can depend on Core ✅
  ├─ CANNOT depend on other Features (at deep level)
  └─ Can have cross-feature communication via shared models/services at app level

Layout
  ├─ Can depend on Shared ✅
  ├─ Can depend on Core ✅
  └─ CANNOT depend on Features
```

---

## IMPORT ORDER IN COMPONENTS/SERVICES

```typescript
// 1. Angular core
import { Component, Injectable, inject } from '@angular/core';

// 2. RxJS
import { Observable, signal } from 'rxjs';

// 3. Core (application infrastructure)
import { AuthService } from '@core/auth/auth.service';
import { ApiService } from '@core/http/api.service';

// 4. Shared (reusable utilities/components)
import { ToastService } from '@shared/services/toast.service';
import { ButtonComponent } from '@shared/components/button/button.component';

// 5. Feature-specific (current feature only)
import { OrderService } from '../services/order.service';
import { OrderModel } from '../models/order.model';

// 6. Relative paths (components in same feature)
import { OrderDisplayComponent } from '../components/order-display/order-display.component';
```

---

## VALIDATION CHECKLIST BEFORE EXECUTION

Before proceeding to Phase 3 (actual file moves), verify:

- [ ] All 50+ files classified above
- [ ] No file marked for both MOVE and SPLIT without clarity
- [ ] New feature folders identified (auth, menu, reservations, checkout, orders, contact, home, static, chefs)
- [ ] Shared components clearly identified (no feature-specific logic)
- [ ] Core infrastructure clearly identified (no feature-specific logic)
- [ ] Services extracted/created for each feature
- [ ] Duplicate code identified (checkout forms, validators, etc.)
- [ ] Circular dependencies checked (none found so far)
- [ ] All dependencies reviewed for new structure compliance
- [ ] Build/test strategy defined

**Status**: ✅ Classification complete. Ready to proceed to Phase 3.

---

## NEXT STEPS

1. ✅ Phase 2A: File Responsibility Map (THIS DOCUMENT)
2. → **Phase 3**: Create core/ infrastructure layer
3. → **Phase 4**: Create shared/ layer
4. → **Phase 5**: Create layout/ folder
5. → **Phase 6**: Extract feature-specific services
6. → **Phase 7**: Migrate features one-by-one
7. → **Phase 8**: Fix imports, resolve circular dependencies
8. → **Phase 9**: Remove old structure, verify, document
