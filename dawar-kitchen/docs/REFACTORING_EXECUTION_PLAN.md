# Angular Architecture Refactoring Execution Plan
## Dawar Kitchen - Efficient Completion Strategy

**Status**: Phases 1-3 Complete (core/ infrastructure created and tested)  
**Current Date**: September 13, 2026  
**Remaining Work**: Phases 4-9

---

## PHASES COMPLETED ✅

### Phase 1: Complete Audit ✅
- Analyzed all 50+ application files
- Mapped responsibilities and dependencies
- Identified architectural problems
- Created comprehensive file responsibility map

### Phase 2A: File Responsibility Map ✅
- Classified every file with action (KEEP/MOVE/MERGE/SPLIT/REFACTOR)
- Documented in: `docs/REFACTORING_PHASE_2A_FILE_MAP.md`
- Ready for execution

### Phase 3: Core Infrastructure Layer ✅
**Created**:
- ✅ `src/app/core/auth/auth.service.ts` (moved with updated imports)
- ✅ `src/app/core/auth/auth.model.ts` (authentication types)
- ✅ `src/app/core/guards/auth.guard.ts` (route protection)
- ✅ `src/app/core/interceptors/auth.interceptor.ts` (JWT token injection)
- ✅ `src/app/core/http/api.service.ts` (HTTP layer)
- ✅ `src/app/core/config/app.config.ts` (application configuration)
- ✅ `src/app/core/config/app.routes.ts` (routing configuration)
- ✅ `src/app/core/index.ts` (barrel export)
- ✅ Updated `src/main.ts` to use new core/config paths
- ✅ Build verified: PASSING ✅

---

## PHASES REMAINING

### Phase 4: Shared Layer
**Action**: Move all cross-cutting, reusable code to `src/app/shared/`

**Models to move** (to `shared/models/`):
```typescript
// Generic/cross-cutting models
- chef.model.ts      → shared/models/chef.model.ts
- seo.model.ts       → shared/models/seo.model.ts
- toast.model.ts     → shared/models/toast.model.ts
```

**Feature-specific models** (to features/*/models/):
```typescript
- auth.model.ts      → core/auth/auth.model.ts ✅ DONE
- cart.model.ts      → features/checkout/models/cart.model.ts
- contact.model.ts   → features/contact/models/contact.model.ts
- menu.model.ts      → features/menu/models/menu.model.ts
- order.model.ts     → features/orders/models/order.model.ts
- reservation.model.ts → features/reservations/models/reservation.model.ts
```

**Services to move** (to `shared/services/`):
```typescript
- theme.service.ts           → shared/services/theme.service.ts
- language.service.ts        → shared/services/language.service.ts (+ REFACTOR: BehaviorSubject → Signal)
- toast.service.ts           → shared/services/toast.service.ts
- seo.service.ts             → shared/services/seo.service.ts
- dropdown-manager.service.ts → shared/services/dropdown-manager.service.ts (+ REFACTOR: Subject → Signal)
```

**Components to move** (to `shared/components/`):
```typescript
- components/animated-background/ → shared/components/animated-background/
- components/toast/ → shared/components/toast/
- components/custom-dropdown/ → shared/components/custom-dropdown/
- components/custom-calendar/ → shared/components/custom-calendar/
- components/language-toggle/ → shared/components/language-toggle/
```

**Update `shared/index.ts`**:
```typescript
export { ThemeService } from './services/theme.service';
export { LanguageService } from './services/language.service';
export { ToastService } from './services/toast.service';
export { SeoService } from './services/seo.service';
export { DropdownManagerService } from './services/dropdown-manager.service';

export { AnimatedBackgroundComponent } from './components/animated-background/animated-background.component';
export { ToastComponent } from './components/toast/toast.component';
export { CustomDropdownComponent } from './components/custom-dropdown/custom-dropdown.component';
export { CustomCalendarComponent } from './components/custom-calendar/custom-calendar.component';
export { LanguageToggleComponent } from './components/language-toggle/language-toggle.component';

export type { Chef, SeoConfig, Toast, ToastType } from './models/index';
```

### Phase 5: Layout Layer
**Action**: Move global shell components to `src/app/layout/`

**Components to move** (to `layout/components/`):
```typescript
- components/header/ → layout/components/header/
- components/footer/ → layout/components/footer/
```

**Update `app.component.ts` imports**:
```typescript
import { HeaderComponent } from '@layout/components/header/header.component';
import { FooterComponent } from '@layout/components/footer/footer.component';
```

**Create `layout/index.ts`**:
```typescript
export { HeaderComponent } from './components/header/header.component';
export { FooterComponent } from './components/footer/footer.component';
```

### Phase 6: Extract Feature-Specific Services
**Action**: Create new services that centralize business logic

**New services to create**:

1. **features/checkout/services/checkout.service.ts**
   ```typescript
   - Manages checkout form state
   - Handles form validation
   - Coordinates Stripe session creation
   - Used by: checkout.component, cart-drawer.component
   ```

2. **features/orders/services/order.service.ts**
   ```typescript
   - Manages order workflow (cart → checkout → confirmation)
   - Coordinates realtime order status tracking
   - Used by: order-confirmed.component, checkout flow
   ```

3. **features/menu/services/menu.service.ts**
   ```typescript
   - Fetches menu items with caching
   - Handles category filtering
   - Used by: menu-page.component, home.component sections
   ```

4. **features/reservations/services/reservation.service.ts**
   ```typescript
   - Manages reservation form state
   - Handles validation and submission
   - Used by: reservations-page.component
   ```

5. **features/contact/services/contact.service.ts**
   ```typescript
   - Manages contact form submission
   - Used by: contact-page.component
   ```

6. **features/chefs/services/chef.service.ts**
   ```typescript
   - Fetches chefs data
   - Caches results
   - Used by: chefs-section.component
   ```

### Phase 7: Migrate Features (One-by-One)
**Action**: Move each feature to `src/app/features/<feature-name>/`

**features/auth/** (Authentication)
```
auth/
├── pages/
│   ├── login.component.ts
│   └── register.component.ts
├── components/
│   ├── auth-modal.component.ts (SPLIT into login-form + register-form)
│   ├── login-form.component.ts
│   └── register-form.component.ts
├── services/
│   └── auth.service.ts (already in core/auth/)
├── models/
│   └── auth.model.ts (already in core/auth/)
└── routes.ts (optional, add to app.routes.ts)
```

**features/menu/** (Menu Management)
```
menu/
├── pages/
│   └── menu-page.component.ts
├── components/
│   ├── menu-display.component.ts (from sections/menu)
│   ├── category-filter.component.ts (from sections/category)
├── services/
│   └── menu.service.ts (NEW)
├── models/
│   └── menu.model.ts
```

**features/checkout/** (Cart & Checkout Flow)
```
checkout/
├── pages/
│   ├── checkout-page.component.ts
│   ├── payment-success-page.component.ts
│   └── payment-cancelled-page.component.ts
├── components/
│   ├── cart-drawer.component.ts (SPLIT into 3 components)
│   ├── cart-display.component.ts (NEW)
│   ├── checkout-form.component.ts (NEW - shared between cart-drawer & checkout)
│   └── order-confirmation.component.ts (NEW)
├── services/
│   ├── cart.service.ts (moved)
│   └── checkout.service.ts (NEW)
├── models/
│   └── cart.model.ts
```

**features/reservations/** (Reservations)
```
reservations/
├── pages/
│   └── reservation-page.component.ts
├── components/
│   └── reservation-form.component.ts (extracted from page)
├── services/
│   └── reservation.service.ts (NEW)
├── models/
│   └── reservation.model.ts
```

**features/orders/** (Orders & Order Tracking)
```
orders/
├── pages/
│   └── order-confirmed-page.component.ts
├── components/
│   └── order-status.component.ts (extracted from page)
├── services/
│   ├── order.service.ts (NEW)
│   └── realtime.service.ts (moved & enhanced)
├── models/
│   └── order.model.ts
```

**features/contact/** (Contact Form)
```
contact/
├── pages/
│   └── contact-page.component.ts
├── components/
│   └── contact-form.component.ts (extracted from page)
├── services/
│   └── contact.service.ts (NEW)
├── models/
│   └── contact.model.ts
```

**features/home/** (Landing Page)
```
home/
├── pages/
│   └── home.component.ts
├── sections/
│   ├── hero-section.component.ts (from sections/hero)
│   ├── chefs-section.component.ts (from sections/chefs)
│   ├── reservation-cta.component.ts (from sections/reservation)
│   ├── cinematic-banner.component.ts (from sections/cinematic-banner)
│   ├── about-section.component.ts (from sections/about)
│   ├── menu-section.component.ts (menu preview section)
│   ├── blog-section.component.ts (from sections/blog - if used)
│   └── locations-section.component.ts (from sections/locations - if used)
```

**features/static/** (Static Pages)
```
static/
├── pages/
│   ├── about-page.component.ts
│   ├── privacy-page.component.ts
│   ├── terms-page.component.ts
│   └── not-found-page.component.ts
```

**features/chefs/** (Chef Management)
```
chefs/
├── components/
│   └── chefs-showcase.component.ts
├── services/
│   └── chef.service.ts (NEW)
├── models/
│   └── chef.model.ts (moved from shared)
```

### Phase 8: Fix Imports & Resolve Circular Dependencies
**Action**: Update all imports throughout the codebase

**Import Standardization**:
```typescript
// 1. Angular core
import { Component, Injectable, inject } from '@angular/core';

// 2. RxJS
import { Observable, signal } from 'rxjs';

// 3. Core infrastructure
import { AuthService } from '@core/auth/auth.service';
import { ApiService } from '@core/http/api.service';

// 4. Shared utilities
import { ToastService } from '@shared/services/toast.service';
import { ButtonComponent } from '@shared/components/button/button.component';

// 5. Feature-specific (current feature only)
import { OrderService } from '../services/order.service';

// 6. Relative paths (same feature)
import { OrderDisplayComponent } from '../components/order-display/order-display.component';
```

**Circular Dependency Audit**:
- Verify: Features → Shared ✅
- Verify: Features → Core ✅
- Verify: Shared ✗→ Features
- Verify: Core ✗→ Features
- Verify: No Feature A → Feature B deep imports

### Phase 9: Final Cleanup & Verification
**Action**: Remove old structure, test, document

1. **Delete old directories** (after all files moved):
   ```
   - src/app/components/ (moved to shared/components or layout/components)
   - src/app/services/ (moved to core/ or shared/services or features/*/services)
   - src/app/pages/ (moved to features/*/pages)
   - src/app/sections/ (moved to features/ or shared/components)
   - src/app/guards/ (moved to core/guards)
   - src/app/interceptors/ (moved to core/interceptors)
   - src/app/models/ (distributed to core/auth, shared/models, features/*/models)
   - src/app/directives/ (moved to shared/directives)
   - Old app.routes.ts, app.config.ts (moved to core/config)
   ```

2. **Verify build**:
   ```bash
   npm run build
   ```
   Expected: ✅ Success (similar bundle size)

3. **Run tests**:
   ```bash
   npm test
   ```
   Expected: ✅ All tests passing

4. **Run E2E** (if available):
   ```bash
   npm run e2e
   ```
   Expected: ✅ All tests passing

5. **Manual functionality verification**:
   - ✅ Home page loads
   - ✅ Menu navigation works
   - ✅ Reservations form works
   - ✅ Checkout flow works
   - ✅ Authentication (login/register) works
   - ✅ Cart operations work
   - ✅ Language toggle (AR/EN) works
   - ✅ RTL layout correct
   - ✅ PWA service worker active
   - ✅ Toast notifications display
   - ✅ Dark/light theme toggle works

6. **Update documentation**:
   ```
   Create: docs/ARCHITECTURE.md
   ```
   Include:
   - New folder structure diagram
   - Core/Shared/Feature/Layout explanation
   - Import rules
   - Where to add new features
   - SOLID decisions made
   - Testing strategy
   - Known limitations

---

## EFFICIENCY NOTES

**To complete this refactoring efficiently**:

1. **Use string replacement** to update imports in bulk across files
2. **Leverage file relocation** with automatic import updates where possible
3. **Group similar moves** (all models together, then all services, etc.)
4. **Build after each phase** to catch errors early
5. **Commit after major milestones** to maintain ability to revert

**Expected Timeline**:
- Phase 4 (Shared): 15-20 minutes
- Phase 5 (Layout): 5 minutes
- Phase 6 (Extract services): 30-40 minutes
- Phase 7 (Migrate features): 60-90 minutes
- Phase 8 (Fix imports): 30-45 minutes
- Phase 9 (Cleanup & verify): 30-45 minutes

**Total**: ~3.5-4 hours of focused execution

---

## CRITICAL SUCCESS FACTORS

1. ✅ **Build must pass after each phase**
2. ✅ **No functionality loss** - all existing features continue working
3. ✅ **Import paths are correct** - no circular dependencies
4. ✅ **Consistent naming** - all files follow Angular conventions
5. ✅ **Tests updated** - all test paths and imports updated
6. ✅ **Documentation clear** - next developer can follow architecture

---

## NEXT IMMEDIATE STEPS

1. Complete Phase 4: Move models to shared/
2. Complete Phase 4: Move services to shared/
3. Complete Phase 4: Move components to shared/
4. Test build: `npm run build`
5. Commit: "chore: migrate shared layer (models, services, components)"
6. Continue to Phase 5

