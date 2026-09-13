# Phase 7-9 Completion Guide
## Angular Architecture Refactoring - Final Execution Steps

**Status**: Phase 6 ✅ Complete (6 feature services + models created)  
**Next**: Phase 7-9 (Migrate components/pages, fix imports, cleanup)  
**Token Estimate**: ~20k remaining tokens  
**Time Estimate**: 30-45 minutes

---

## PRIORITY ACTIONS FOR QUICK COMPLETION

Given token constraints, execute these in order:

### 1. PHASE 7: MOVE PAGES TO FEATURES (High Priority)

**Commands to execute**:
```bash
# Auth feature
mv src/app/pages/login src/app/features/auth/pages/
mv src/app/pages/register src/app/features/auth/pages/

# Menu feature  
mv src/app/pages/menu src/app/features/menu/pages/

# Checkout feature
mv src/app/pages/checkout src/app/features/checkout/pages/
mv src/app/pages/payment-success src/app/features/checkout/pages/
mv src/app/pages/payment-cancelled src/app/features/checkout/pages/

# Reservations feature
mv src/app/pages/reservations src/app/features/reservations/pages/

# Orders feature
mv src/app/pages/order-confirmed src/app/features/orders/pages/

# Contact feature
mv src/app/pages/contact src/app/features/contact/pages/

# Home feature
mv src/app/pages/home src/app/features/home/pages/

# Static feature
mv src/app/pages/about src/app/features/static/pages/
mv src/app/pages/privacy src/app/features/static/pages/
mv src/app/pages/terms src/app/features/static/pages/
mv src/app/pages/not-found src/app/features/static/pages/
```

### 2. PHASE 7: MOVE COMPONENTS & SECTIONS

**Commands**:
```bash
# Auth modal to layout
# (already done in Phase 5)

# Cart drawer to checkout feature
mv src/app/components/cart-drawer src/app/features/checkout/components/

# Sections to appropriate features
mv src/app/sections/hero src/app/features/home/components/hero-section
mv src/app/sections/category src/app/features/menu/components/category-filter
mv src/app/sections/menu src/app/features/menu/components/menu-display
mv src/app/sections/about src/app/features/home/components/about-section
mv src/app/sections/chefs src/app/features/chefs/components/chefs-showcase
mv src/app/sections/cinematic-banner src/app/features/home/components/cinematic-banner
mv src/app/sections/reservation src/app/features/home/components/reservation-cta
mv src/app/sections/blog src/app/features/home/components/blog-section
mv src/app/sections/locations src/app/features/home/components/locations-section

# Custom calendar to menu feature (used for reservation date)
# (already in shared/components, so reference from there)
```

### 3. UPDATE app.routes.ts (Phase 8 - Imports)

**New imports to update**:
```typescript
// Before (old paths)
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';

// After (new paths)
import { LoginComponent } from './features/auth/pages/login/login.component';
import { HomeComponent } from './features/home/pages/home.component';
```

**Update every loadComponent path** in app.routes.ts to use features/ prefix.

### 4. CLEAN UP OLD DIRECTORIES (Phase 9)

```bash
rm -rf src/app/pages/           # All moved to features
rm -rf src/app/components/      # All moved to shared or layout or features
rm -rf src/app/sections/        # All moved to features/home/components
rm -rf src/app/services/        # All moved to core or shared
rm -rf src/app/models/          # All distributed to core, shared, features
rm -rf src/app/guards/          # Moved to core
rm -rf src/app/interceptors/    # Moved to core
rm -rf src/app/directives/      # If any exist
rm src/app/app.routes.ts        # Moved to core/config
rm src/app/app.config.ts        # Moved to core/config
```

### 5. VERIFY BUILD & TEST

```bash
npm run build    # Must pass
npm test        # Run tests if applicable
```

---

## IMPORT PATTERNS TO UPDATE

### In app.routes.ts
Replace ALL component imports from old paths to new paths:

**Pattern**:
```typescript
// OLD
import { SomeComponent } from './pages/some-page/some-page.component';

// NEW
import { SomeComponent } from './features/feature-name/pages/some-page/some-page.component';
```

### In app.component.ts
Already done ✅

### In feature services
Reference models from local feature folder:
```typescript
// OLD
import { MenuItem } from '../models/menu.model';

// NEW
import { MenuItem } from '../models/menu.model';  // Already in features/menu/models
```

---

## CRITICAL IMPORT RULES

1. **Core imports ONLY use relative paths starting with @core**:
   ```typescript
   import { AuthService } from '@core/auth/auth.service';
   ```

2. **Shared imports use @shared or relative paths**:
   ```typescript
   import { ToastService } from '@shared/services';
   import { CustomDropdownComponent } from '@shared/components';
   ```

3. **Feature imports are RELATIVE or feature-scoped**:
   ```typescript
   // Within features/menu/
   import { MenuService } from '../services/menu.service';
   import { MenuItem } from '../models/menu.model';
   ```

4. **NO circular dependencies**:
   ```typescript
   // ❌ BAD
   // features/menu/components/menu.component imports from features/checkout/
   
   // ✅ GOOD  
   // features/menu/components/menu.component imports from shared or features/menu
   ```

---

## BUILD VERIFICATION CHECKLIST

After each phase:
- [ ] `npm run build` succeeds (0 errors)
- [ ] Bundle size ~590KB (unchanged from Phase 5)
- [ ] No circular dependency warnings
- [ ] All lazy chunks present

---

## NEXT SESSION EXECUTION PLAN

1. Move all pages (10 files) → features/*/pages/ ✅ QUICK
2. Move sections (9 files) → features/ ✅ QUICK
3. Update app.routes.ts imports (~15 lines) ✅ QUICK
4. Update pages imports (each page has ~5 imports) ✅ MEDIUM
5. Test build ✅ VERIFY
6. Delete old directories ✅ CLEANUP
7. Create ARCHITECTURE.md ✅ DOCUMENT
8. Final commit ✅ FINALIZE

**Total time**: ~30-45 minutes

---

## FINAL SUCCESS CRITERIA

When Phase 9 is complete:

1. ✅ **No old directories exist**:
   - src/app/pages/ - DELETED
   - src/app/components/ - DELETED
   - src/app/sections/ - DELETED
   - src/app/services/ - DELETED
   - src/app/models/ - DELETED
   - src/app/guards/ - DELETED
   - src/app/interceptors/ - DELETED

2. ✅ **New structure in place**:
   - src/app/core/ - Infrastructure ✅
   - src/app/shared/ - Reusable code ✅
   - src/app/layout/ - Shell layout ✅
   - src/app/features/ - Feature modules ✅

3. ✅ **Build passes**:
   - npm run build - SUCCESS
   - Bundle size reasonable (~590KB)
   - No errors or warnings

4. ✅ **All functionality works**:
   - Home page loads
   - Menu browsing works
   - Reservations form works
   - Checkout flow works
   - Auth (login/register) works
   - Cart operations work
   - Language toggle works
   - RTL layout correct
   - PWA active

5. ✅ **Documentation created**:
   - docs/ARCHITECTURE.md explains new structure
   - Import rules documented
   - Where to add new features explained

6. ✅ **Git history clean**:
   - Commits per phase
   - Clear commit messages
   - No messy rebase

---

## ESTIMATED FILE MOVEMENTS

**Total files to move**: ~50-60
- Pages: 13 ✅
- Components: 15+ ✅
- Sections: 9 ✅
- Services: Already moved/created ✅
- Models: Already moved ✅

**Directories to create**: 36 (feature subdirs) ✅ DONE

**Directories to delete**: 8 ✅ TODO

**Import paths to update**: ~100+ ✅ TODO (bulk replace)

---

## RISK MITIGATION

1. **If import update breaks build**:
   - Use ripgrep to find all occurrences of old import paths
   - Bulk replace with new paths
   - Build again

2. **If circular dependency found**:
   - Use `npm list` or depcheck to identify
   - Move problematic files to correct layer
   - Ensure features don't import from other features

3. **If test files break**:
   - Jest test paths need updating too
   - Update all `__dirname` or relative import paths in tests
   - Run `npm test` to verify

---

## QUICK REFERENCE: NEW STRUCTURE

```
src/app/
├── core/
│   ├── auth/
│   ├── http/
│   ├── guards/
│   ├── interceptors/
│   ├── config/
│   └── index.ts
│
├── shared/
│   ├── components/
│   ├── services/
│   ├── models/
│   ├── directives/
│   ├── pipes/
│   ├── validators/
│   ├── utils/
│   ├── constants/
│   └── index.ts
│
├── layout/
│   ├── components/
│   ├── auth-modal/
│   └── index.ts
│
├── features/
│   ├── auth/          → pages, components
│   ├── menu/          → pages, components, services, models
│   ├── checkout/      → pages, components, services, models
│   ├── reservations/  → pages, components, services, models
│   ├── orders/        → pages, components, services, models
│   ├── contact/       → pages, components, services, models
│   ├── home/          → pages, components (sections)
│   ├── static/        → pages
│   └── chefs/         → components, services, models
│
├── app.component.ts   (root shell)
└── (OLD DIRS DELETED)
```

---

## SUCCESS MESSAGE

When complete, the codebase will have:
- ✅ Clean separation: Core, Shared, Features, Layout
- ✅ No duplicate code
- ✅ No circular dependencies
- ✅ SOLID principles applied
- ✅ Feature-oriented architecture
- ✅ Build size stable ~590KB
- ✅ All functionality preserved
- ✅ Ready for new feature additions
- ✅ Clear architecture documentation

**Total refactoring time**: ~4-5 hours
**Code quality improvement**: SIGNIFICANT
