# Dawar Kitchen Angular Architecture

## Overview

The Dawar Kitchen frontend has been refactored from a flat, monolithic structure into a **clean, feature-oriented, SOLID-compliant architecture** following Angular best practices.

**Status**: ✅ Phase 1-7 Complete | Phase 8-9 In Progress  
**Date**: September 13, 2026  
**Angular Version**: 18  
**Architecture Pattern**: Feature-Oriented with Core/Shared/Layout Separation

---

## Architecture Principles

### 1. **Feature-Oriented Structure**
The application is organized around business features, not technical layers. Each feature owns its pages, components, services, and models.

### 2. **Core/Shared/Feature Separation**
```
┌─────────────────────────────────────┐
│        Application (App Router)      │
└─────────────────────────────────────┘
              ↓
    ┌─────────────────────┐
    │    Features Layer   │  (Business domains)
    └─────────────────────┘
       ↓          ↓
   ┌────────┐  ┌──────────┐
   │ Shared │  │ Layout   │  (Cross-cutting & shell)
   └────────┘  └──────────┘
       ↓
   ┌────────┐
   │ Core   │  (Infrastructure only)
   └────────┘
```

**Import Direction** (top-down):
```
Features → Shared ✅
Features → Core ✅
Shared → Features ❌
Core → Features ❌
```

### 3. **Single Responsibility Principle**
- Components: UI presentation only
- Services: Business logic and state
- Guards: Route protection
- Interceptors: HTTP concerns only

### 4. **SOLID Applied**
- **S**ingle Responsibility: Each service/component has one reason to change
- **O**pen/Closed: New features don't require modifying existing code
- **L**iskov Substitution: No unnecessary inheritance
- **I**nterface Segregation: Services expose focused APIs
- **D**ependency Inversion: All services injected via DI, no `new` keyword

---

## Project Structure

```
src/app/
│
├── core/                          # Application infrastructure (no business logic)
│   ├── auth/                      # Authentication infrastructure
│   │   ├── auth.service.ts        # Auth state, login/logout, token mgmt
│   │   └── auth.model.ts          # AuthSession, LoginResponse interfaces
│   │
│   ├── http/                      # HTTP communication infrastructure
│   │   └── api.service.ts         # Generic HTTP client (pure infrastructure)
│   │
│   ├── guards/                    # Route protection infrastructure
│   │   └── auth.guard.ts          # Require authenticated users
│   │
│   ├── interceptors/              # HTTP pipeline infrastructure
│   │   └── auth.interceptor.ts    # Inject JWT bearer token
│   │
│   ├── config/                    # Application configuration
│   │   ├── app.routes.ts          # Routing configuration
│   │   └── app.config.ts          # Providers & bootstrapping
│   │
│   └── index.ts                   # Public exports
│
├── shared/                        # Reusable code (no feature-specific logic)
│   ├── components/                # Generic UI components
│   │   ├── animated-background/   # Visual effect overlay
│   │   ├── toast/                 # Notification display
│   │   ├── custom-dropdown/       # Generic dropdown wrapper
│   │   ├── custom-calendar/       # Date picker widget
│   │   ├── language-toggle/       # Language switcher (EN/AR)
│   │   └── index.ts               # Public exports
│   │
│   ├── services/                  # Cross-cutting concerns
│   │   ├── theme.service.ts       # Dark/light theme state
│   │   ├── language.service.ts    # i18n language state (+ RTL)
│   │   ├── toast.service.ts       # Notification queue
│   │   ├── seo.service.ts         # Page metadata (title, OG tags)
│   │   ├── dropdown-manager.service.ts  # Dropdown UI coordination
│   │   └── index.ts               # Public exports
│   │
│   ├── models/                    # Generic types
│   │   ├── toast.model.ts         # Toast notification types
│   │   ├── seo.model.ts           # SEO config interface
│   │   └── index.ts               # Public exports
│   │
│   ├── directives/                # Reusable directives
│   │   ├── image-optimization.directive.ts
│   │   └── scroll-reveal.directive.ts
│   │
│   ├── validators/                # Custom form validators (if any)
│   ├── pipes/                     # Custom pipes (if any)
│   ├── utils/                     # Utility functions
│   ├── constants/                 # Global constants
│   └── index.ts                   # Main barrel export
│
├── layout/                        # Global shell layout (not a feature)
│   ├── components/
│   │   ├── header/                # Main navigation bar
│   │   └── footer/                # Footer navigation
│   │
│   ├── auth-modal/                # Global auth modal (login/register)
│   └── index.ts                   # Public exports
│
├── features/                      # Business features (domain-driven)
│   │
│   ├── auth/                      # Authentication feature
│   │   ├── pages/
│   │   │   ├── login/             # Login page
│   │   │   └── register/          # Registration page
│   │   ├── models/
│   │   │   └── auth.model.ts      # (references core/auth)
│   │   └── services/              # (uses core/auth)
│   │
│   ├── menu/                      # Menu browsing feature
│   │   ├── pages/
│   │   │   └── menu/              # Menu page with filtering
│   │   ├── components/
│   │   │   ├── menu-display/      # Menu items grid
│   │   │   └── category-filter/   # Category filter
│   │   ├── services/
│   │   │   └── menu.service.ts    # Menu fetching + caching
│   │   └── models/
│   │       └── menu.model.ts      # MenuItem interface
│   │
│   ├── checkout/                  # Cart & checkout feature
│   │   ├── pages/
│   │   │   ├── checkout/          # Checkout form page
│   │   │   ├── payment-success/   # Payment success page
│   │   │   └── payment-cancelled/ # Payment cancel page
│   │   ├── components/
│   │   │   └── cart-drawer/       # Cart drawer modal
│   │   ├── services/
│   │   │   ├── cart.service.ts    # Cart state management
│   │   │   └── checkout.service.ts # Checkout form + payment
│   │   └── models/
│   │       ├── cart.model.ts      # CartItem interface
│   │       └── order.model.ts     # Order/Checkout types
│   │
│   ├── reservations/              # Table reservation feature
│   │   ├── pages/
│   │   │   └── reservations/      # Reservation form page
│   │   ├── services/
│   │   │   └── reservation.service.ts # Reservation submission
│   │   └── models/
│   │       └── reservation.model.ts # Reservation types
│   │
│   ├── orders/                    # Order tracking feature
│   │   ├── pages/
│   │   │   └── order-confirmed/   # Order confirmation page
│   │   ├── services/
│   │   │   ├── order.service.ts   # Order state + workflow
│   │   │   └── realtime.service.ts # WebSocket order updates
│   │   └── models/
│   │       └── order.model.ts     # Order response types
│   │
│   ├── contact/                   # Contact form feature
│   │   ├── pages/
│   │   │   └── contact/           # Contact form page
│   │   ├── services/
│   │   │   └── contact.service.ts # Contact submission
│   │   └── models/
│   │       └── contact.model.ts   # Contact types
│   │
│   ├── home/                      # Landing page feature
│   │   ├── pages/
│   │   │   └── home/              # Landing page
│   │   ├── components/            # Page sections
│   │   │   ├── hero-section/      # Hero banner
│   │   │   ├── menu-display/      # Menu preview
│   │   │   ├── about-section/     # About section
│   │   │   ├── chefs-section/     # Chefs showcase
│   │   │   ├── cinematic-banner/  # Visual banner
│   │   │   ├── reservation-cta/   # Reservation CTA
│   │   │   ├── blog-section/      # Blog posts
│   │   │   └── locations-section/ # Locations map
│   │   └── services/              # (if needed)
│   │
│   ├── static/                    # Static pages feature
│   │   ├── pages/
│   │   │   ├── about/             # About page
│   │   │   ├── privacy/           # Privacy policy
│   │   │   ├── terms/             # Terms & conditions
│   │   │   └── not-found/         # 404 page
│   │   └── models/
│   │
│   ├── chefs/                     # Chef showcase feature
│   │   ├── components/
│   │   │   └── chefs-showcase/    # Chefs display
│   │   ├── services/
│   │   │   └── chef.service.ts    # Fetch chefs + caching
│   │   └── models/
│   │       └── chef.model.ts      # Chef interface
│   │
│   └── [future features]
│
├── app.component.ts               # Root shell component
└── index.html                     # Entry point
```

---

## Import Rules & Best Practices

### Correct Import Patterns

```typescript
// 1. Angular & Third-party
import { Component, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

// 2. Core infrastructure (absolute paths with @core alias)
import { AuthService } from '@core/auth/auth.service';
import { ApiService } from '@core/http/api.service';

// 3. Shared utilities (absolute paths with @shared alias)
import { ToastService, SeoService } from '@shared/services';
import { CustomDropdownComponent } from '@shared/components';
import { Toast } from '@shared/models';

// 4. Feature services (relative paths from same feature)
import { MenuService } from '../services/menu.service';
import { MenuItem } from '../models/menu.model';

// 5. Layout (if needed)
import { HeaderComponent } from '@layout/components/header/header.component';

// ❌ NEVER:
import { SomeService } from '../services/some.service';  // Wrong path
import { CartService } from 'src/app/features/checkout/services/cart.service'; // Wrong: absolute in component
```

### Path Aliases (tsconfig.json)

```json
{
  "baseUrl": "./src/app",
  "paths": {
    "@core/*": ["core/*"],
    "@shared/*": ["shared/*"],
    "@layout/*": ["layout/*"],
    "@features/*": ["features/*"]
  }
}
```

---

## Where to Add New Features

### New Feature: Add Restaurant Menu Categories

1. **Decide**: Is this part of an existing feature (menu) or new feature (categories)?
   - **If existing**: Add to `features/menu/services/menu.service.ts`
   - **If new**: Create `features/categories/`

2. **Create structure**:
   ```
   features/categories/
   ├── pages/
   │   └── categories/categories-page.component.ts
   ├── components/
   │   └── category-item/category-item.component.ts
   ├── services/
   │   └── category.service.ts
   ├── models/
   │   └── category.model.ts
   └── routes.ts (optional)
   ```

3. **Add to app.routes.ts**:
   ```typescript
   {
     path: 'categories',
     loadComponent: () => import('../../features/categories/pages/categories/categories-page.component')
       .then(m => m.CategoriesPageComponent)
   }
   ```

4. **Import rules**:
   - Import shared components/services
   - Import core services (auth, api)
   - Do NOT import from other features

---

## State Management

### Current Approach
- **Signals** for local/cross-cutting state (theme, language, toast)
- **RxJS BehaviorSubject** for async operations
- **No global state manager** (NgRx) needed
- **localStorage** for persistence (cart, language, theme)

### State Service Pattern

```typescript
@Injectable({ providedIn: 'root' })
export class CartService {
  // Use signals for reactive state
  private items$ = signal<CartItem[]>(this.loadCart());
  readonly items = this.items$;  // Public read-only
  
  constructor() {
    // Sync to localStorage on changes
    effect(() => {
      localStorage.setItem('cart', JSON.stringify(this.items$()));
    });
  }
  
  addItem(item: CartItem) {
    this.items$.update(items => [...items, item]);
  }
}
```

---

## Authentication Flow

```
┌─────────────────┐
│  Login Page     │
└────────┬────────┘
         │ AuthService.login()
         ↓
┌─────────────────┐
│  AuthService    │ → POST /api/auth/login → Backend
│  (stores token) │ → localStorage['nn_session']
└────────┬────────┘
         │ AuthService.currentUser signal
         ↓
┌─────────────────┐
│  AuthInterceptor│ → Attaches Bearer token to HTTP requests
└────────┬────────┘
         │
┌─────────────────┐
│  AuthGuard      │ → Protects /order-confirmed route
└─────────────────┘
```

**Key Points**:
- ✅ AuthService manages state
- ✅ AuthInterceptor adds token to requests
- ✅ AuthGuard protects routes
- ⚠️ Backend is the real security boundary (not Angular guards)

---

## Testing Strategy

### Unit Tests
- Test services in isolation with mocks
- Test component logic (not templates)
- Mock dependencies via TestBed providers

### Component Tests
- Test user interactions
- Verify output events
- Mock services

### E2E Tests (Cypress)
- Test critical user journeys
- Auth → Menu → Checkout → Payment → Confirmation
- Language/RTL switching

---

## Performance Considerations

### Bundle Size
- Current: ~591 kB (uncompressed)
- Gzipped: ~148 kB
- Lazy loading: 18 feature chunks

### Optimization Techniques
1. **Lazy loading**: All routes except home
2. **Preloading**: menu, reservations (high priority)
3. **OnPush detection**: Stateless components
4. **TrackBy**: Repeat lists use item IDs
5. **SharedReplay**: API calls cached

---

## i18n & RTL

### Language Support
- English (en)
- Arabic (ar)

### Implementation
- `LanguageService`: Manages language state + document direction
- Translation files: `assets/i18n/{en,ar}.json`
- `TranslateModule`: All templates use `| translate` pipe
- Document RTL: Service sets `dir="rtl"` on `<html>`

---

## PWA & Service Worker

### Enabled
- Service worker: `ngsw-worker.js`
- Offline support: cached assets
- Install prompt: offered on compatible browsers

### Configuration
- `ngsw-config.json`: Cache strategies
- `manifest.json`: App metadata
- Home screen icon: `assets/icon-*`

---

## Build & Deployment

### Build Command
```bash
npm run build
```
Outputs: `dist/dawar-kitchen/`

### Deployment
- Vercel (configured in `vercel.json`)
- Environment variables: `.env`
- Service worker registers on stable

---

## Known Limitations & Future Work

1. **Realtime updates**: WebSocket stub only, needs backend integration
2. **Error handling**: Global error handler not implemented
3. **Caching**: Menu fetched every time (should cache server-side)
4. **Pagination**: No pagination for large menu lists
5. **Accessibility**: ARIA attributes minimal, needs audit
6. **Tests**: Test coverage ~40% (needs expansion)

---

## SOLID Decisions Made

### Single Responsibility
- ✅ `CartService`: Only manages cart state
- ✅ `MenuService`: Only fetches/caches menu
- ✅ Components: Only present UI

### Open/Closed
- ✅ New features added without modifying routing
- ✅ Services extended via providers, not modification

### Liskov Substitution
- ✅ No inheritance used (prefer composition)

### Interface Segregation
- ✅ Services expose focused APIs
- ✅ No god interfaces

### Dependency Inversion
- ✅ All services injected (no `new` keyword)
- ✅ Features depend on abstractions (core, shared)

---

## Critical Files

| File | Purpose |
|------|---------|
| `core/config/app.routes.ts` | All routing definitions |
| `core/config/app.config.ts` | Application providers |
| `app.component.ts` | Root shell layout |
| `tsconfig.json` | Path aliases for imports |
| `shared/index.ts` | Barrel export for shared |
| `features/*/index.ts` | Feature public API |

---

## Version History

- **v1.1.3** (Sept 13, 2026): Architecture refactored to feature-oriented with core/shared/layout separation
- **v1.1.0** (Previous): Monolithic structure

---

## Questions & Answers

**Q: Where should I add a new component?**
A: If it's reusable across features → `shared/components/`. If it's feature-specific → `features/feature-name/components/`.

**Q: How do I add a new service?**
A: If it's global/cross-cutting → `shared/services/`. If it's feature-specific → `features/feature-name/services/`.

**Q: Can features import from other features?**
A: No. Features should only import from core and shared. Cross-feature communication via shared models.

**Q: Why no NgRx?**
A: Current state (Signals + localStorage) is simpler and sufficient. Add NgRx only if state becomes unmanageable.

---

**Last Updated**: September 13, 2026  
**Architecture Owner**: [Your Team]  
**Review**: Quarterly
