# Agent Prompt: Deep Clean Architecture & SOLID Hardening for Dawar Kitchen Backend

**Target:** `api-server/src` (NaarNoor.* projects)  
**Repo:** https://github.com/Mostafa-SAID7/Dawar-Kitchen-Full  
**Focus:** Make the backend **solid, clean, and deeply layered** — Domain-centric, CQRS + MediatR, true Clean Architecture, full SOLID compliance.  
**Date context:** Post-refactor (Auth moved to Application via MediatR). Audit defects partially addressed; many structural issues remain.

---

## 1. Current Architecture Snapshot (What Exists)

```
api-server/src/
├── NaarNoor.API/                 # Presentation (Controllers, Middleware, Configuration)
├── NaarNoor.Application/         # Use cases (CQRS Features, DTOs, Interfaces, Behaviours)
├── NaarNoor.Domain/              # Entities, Enums, ValueObjects, BaseEntity
└── NaarNoor.Infrastructure/      # EF Core, Repositories, External services (Stripe, Supabase, JWT, User)
```

**Good foundations already present:**
- Project separation matches Clean Architecture layers.
- MediatR + FluentValidation pipeline (`ValidationBehaviour`).
- Feature folders under Application (`Features/{Feature}/{Commands|Queries}/...`).
- Generic `IRepository<T>` + `IUnitOfWork` + `IApplicationDbContext`.
- Some domain behaviour on `Reservation` (state machine `TransitionTo`, `IsInValidState`).
- Value objects `Money` and `TimeSlot` exist (but are **unused**).
- Server-side price re-computation in order creation (security positive).
- Recent refactor moved Auth to Application layer and consolidated some service interfaces.

**Critical remaining problems (must fix):**

| # | Issue | Severity | Location / Symptom |
|---|-------|----------|--------------------|
| 1 | **Anemic Domain Model** | Critical | Entities are mostly POCOs with public setters. `Money`/`TimeSlot` never used. Business rules live in handlers or services. |
| 2 | **EF Core leak into Application** | Critical | Handlers import `Microsoft.EntityFrameworkCore` and call `.ToListAsync()`, `.Where()`, `.Query()`. Application depends on persistence technology. |
| 3 | **IQueryable exposure** | High | `IRepository.Query()` returns `IQueryable<T>` → Application can build arbitrary queries and is coupled to EF. |
| 4 | **Dual persistence abstraction** | High | Both `IUnitOfWork` **and** `IApplicationDbContext` used side-by-side (e.g. Stripe handlers inject both). Inconsistent. |
| 5 | **Incomplete vertical slices** | High | `Review` entity exists + migration, but **no** Application Features (no Commands/Queries/Handlers). No ReviewsController. |
| 6 | **Unit of Work incomplete** | Medium | `IUnitOfWork` missing `Reviews` (and possibly `Users`). Lazy repo fields only for a subset. |
| 7 | **Transaction boundaries weak** | Medium | `CreateOrder` does two separate `SaveChangesAsync` calls (Order then OrderItems) → risk of partial persist. |
| 8 | **Namespace inconsistency** | Medium | Some Features use `NaarNoor.Application.Orders...`, others `NaarNoor.Application.Features.Orders...`. |
| 9 | **Service interfaces vs Domain** | Medium | Auth/JWT/Stripe live as Application.Services interfaces implemented in Infrastructure — acceptable, but domain rules should not be pushed into them. |
| 10 | **No domain events / rich aggregates** | Medium | Order + OrderItems is a natural aggregate but not modelled as one. No domain events for OrderCreated, ReservationConfirmed, etc. |
| 11 | **Controller request DTOs mixed** | Low-Medium | Some request classes live inside controller files; mapping is ad-hoc. Prefer Application DTOs or dedicated API contracts. |
| 12 | **CacheService location** | Low | Lives under Application but uses DistributedCache — fine if abstracted; ensure no Infrastructure types leak. |

---

## 2. Target Architecture (What You Must Achieve)

### Dependency Rule (strict)
```
API  →  Application  →  Domain
         ↑
Infrastructure implements Application interfaces and depends on Domain
```
- **Never** let API or Application reference Infrastructure types/namespaces.
- Application must **not** reference `Microsoft.EntityFrameworkCore` (except possibly in tests).
- Domain must have **zero** dependencies on other projects or external frameworks.

### Folder & Layer Responsibilities

**Domain (`NaarNoor.Domain`)**
- Entities with **behaviour** (methods that enforce invariants).
- Value Objects (`Money`, `TimeSlot`, maybe `Email`, `PhoneNumber`, `Address`).
- Enums, Domain Exceptions, Domain Events (optional but recommended).
- Aggregates (e.g. `Order` owns `OrderItem` collection; factory methods).
- No DTOs, no EF attributes, no MediatR.

**Application (`NaarNoor.Application`)**
- Feature folders only: `Features/{Name}/{Commands|Queries}/{UseCaseName}/`.
- Commands / Queries / Handlers / Validators.
- Application interfaces: `IRepository` (or better specific read/write ports), `IUnitOfWork`, service ports (`IJwtService`, `IStripeService`, …).
- DTOs / Result models used by handlers.
- Pipeline behaviours (Validation, Logging, Transaction, Caching).
- **No** EF Core, no concrete DbContext, no Infrastructure namespaces.

**Infrastructure (`NaarNoor.Infrastructure`)**
- `ApplicationDbContext`, configurations, migrations, seeders.
- Concrete `Repository` / `UnitOfWork` implementations.
- External service implementations (Stripe, Supabase, JWT, UserService).
- DI registration only.

**API (`NaarNoor.API`)**
- Thin controllers that only map HTTP → MediatR command/query and map result → HTTP response.
- Middleware, configuration, Swagger, health.
- Request/response contracts can stay here **or** be thin wrappers around Application DTOs.

---

## 3. SOLID Principles — Concrete Enforcement Rules

### Single Responsibility (SRP)
- One handler = one use case.
- Controllers do **only** HTTP concerns (status codes, routing, auth attributes).
- Domain entity methods only enforce that entity’s invariants.
- Do not put reporting calculations, revenue aggregation, or Stripe session building inside controllers or fat services.

### Open/Closed (OCP)
- Prefer extending via new Commands/Queries/Behaviours rather than modifying existing handlers.
- Use strategy or domain events for side-effects (email, realtime) instead of hard-coding inside handlers.

### Liskov Substitution (LSP)
- Any implementation of `IRepository<T>` or service interfaces must be substitutable without changing handler behaviour.
- Avoid “optional” methods that throw `NotImplementedException`.

### Interface Segregation (ISP)
- Prefer small, focused interfaces over a god `IUnitOfWork` that grows forever.
- Consider splitting read vs write (CQRS-friendly): `IOrderReadRepository`, `IOrderWriteRepository` **or** keep UoW but make it complete and thin.
- Do not force handlers to depend on `IApplicationDbContext` when they only need a few repositories.

### Dependency Inversion (DIP)
- Application defines interfaces; Infrastructure implements them.
- Handlers depend only on abstractions (`IUnitOfWork`, `IStripeService`, etc.).
- **Eliminate** direct `IApplicationDbContext` injection in handlers where possible; route everything through repository/UoW abstractions.

---

## 4. Critical Fixes — Ordered Work Plan for the Agent

Execute in this order. Do not skip layers.

### Phase 0 — Inventory & Consistency (short)
1. Standardise namespaces: everything under `NaarNoor.Application.Features.{Feature}.{Commands|Queries}.{Name}`.
2. Confirm Auth is fully MediatR-driven (already done in latest code) — remove any residual direct Infrastructure calls from API.
3. Delete or archive outdated audit claims that no longer apply (Auth layer bypass is fixed).

### Phase 1 — Stop EF Leak (Critical)
1. **Change `IRepository<T>`** so it no longer exposes raw `IQueryable<T>` to Application, **or** introduce a clear boundary:
   - Preferred: keep `Query()` only if you also introduce specification/pattern or explicit methods (`GetAvailableMenuItemsAsync`, `GetByIdsAsync`, etc.).
   - Better long-term: replace free `IQueryable` with explicit query methods or a specification interface that Infrastructure translates.
2. **Remove all `using Microsoft.EntityFrameworkCore;` from Application project.**
   - Move any `.ToListAsync()` / `.FirstOrDefaultAsync()` into Infrastructure or behind repository methods that return `Task<List<T>>` / `Task<T?>`.
3. Make `IUnitOfWork` the **single** persistence entry point for write use-cases.
4. Either:
   - Expand `IUnitOfWork` to include `IRepository<Review> Reviews` (and `Users` if needed), **or**
   - Introduce dedicated repositories per aggregate and drop the dual `IApplicationDbContext` usage in handlers.
5. Fix `CreateOrderCommandHandler` (and similar) to use **one** `SaveChangesAsync` inside a proper transaction scope (or rely on a single unit-of-work save after building the full aggregate).

### Phase 2 — Enrich the Domain (Critical for “strong structure”)
1. **Use the existing Value Objects:**
   - Change `MenuItem.Price`, `Order.TotalAmount`, `OrderItem.UnitPrice` to use `Money` (or keep decimal for simplicity but wrap calculations with `Money` where currency matters).
   - Change Reservation time modelling to use `TimeSlot` where overlapping checks are needed.
2. Make entities richer:
   - `Order` becomes an aggregate root:
     - Private `List<OrderItem> _items` + public read-only collection.
     - Factory `Order.Create(...)` that validates customer data, type, and items.
     - Method `AddItem(MenuItem, quantity)` that uses server price and updates total.
     - Method `MarkPaid(string stripeSessionId)`, `Cancel()`, `TransitionStatus(...)` with state machine similar to Reservation.
   - `Reservation` already has good behaviour — keep and expand if needed (overlap checks via `TimeSlot`).
   - `MenuItem` — methods `MarkUnavailable()`, `UpdatePrice(Money)`, etc.
3. Add domain exceptions (`OrderDomainException`, `ReservationDomainException`) instead of generic `InvalidOperationException` for business rule violations.
4. (Recommended) Introduce simple domain events (`OrderCreatedEvent`, `ReservationConfirmedEvent`) and a MediatR notification or outbox later.

### Phase 3 — Complete Vertical Slices
1. **Review feature** (entity already exists + migration `20260913000000_AddReviewEntity`):
   - `Features/Reviews/Commands/CreateReview/`
   - `Features/Reviews/Queries/GetApprovedReviews/`
   - `Features/Reviews/Commands/ApproveReview/` (admin)
   - Wire controller + validation.
2. Ensure every public controller endpoint has a corresponding Command or Query (no business logic left in controllers).
3. Add missing validators for every command that accepts user input.

### Phase 4 — Repository & Unit-of-Work Hardening
1. Make `UnitOfWork` complete (all aggregates).
2. Prefer constructor injection of repositories over lazy `??=` fields if it improves testability.
3. Consider adding a transactional behaviour or explicit `IUnitOfWork.BeginTransactionAsync` if multi-aggregate consistency is required.
4. Keep EF configurations and migrations only in Infrastructure.

### Phase 5 — API Layer Cleanup
1. Controllers stay thin: create command/query → `_mediator.Send` → map to `IActionResult`.
2. Move any remaining request classes that contain business rules into Application.
3. Ensure consistent status codes and ProblemDetails / exception middleware mapping for domain vs validation exceptions.
4. Keep Swagger and health endpoints as-is (already good).

### Phase 6 — Testing & Guardrails
1. Domain unit tests for aggregate behaviour and state machines (no EF).
2. Application handler tests with mocked `IUnitOfWork` / service interfaces (no real DB).
3. Infrastructure integration tests against Testcontainers or in-memory only where necessary.
4. Add an architecture test (e.g. NetArchTest or custom) that fails if:
   - Application references Infrastructure or EF Core.
   - Domain references anything outside itself.
   - API references Infrastructure (except DI composition root).

---

## 5. Concrete Code Patterns the Agent Must Follow

### Handler shape (after fixes)
```csharp
// Application — NO Microsoft.EntityFrameworkCore
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    // only Application abstractions

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var menuItems = await _uow.MenuItems.GetAvailableByIdsAsync(
            request.Items.Select(i => i.MenuItemId), ct);

        // domain logic
        var order = Order.Create(...); // factory
        foreach (var item in ...)
            order.AddItem(...);

        _uow.Orders.Add(order);
        await _uow.SaveChangesAsync(ct); // single save
        return order.Id;
    }
}
```

### Repository contract direction
```csharp
// Prefer explicit methods over free IQueryable for Application consumers
public interface IMenuItemRepository
{
    Task<IReadOnlyList<MenuItem>> GetAvailableByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct);
    // ...
}
```
(You may keep a generic repository for simple CRUD, but complex queries belong behind named methods or specifications implemented in Infrastructure.)

### Domain aggregate sketch
```csharp
public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Money Total { get; private set; }

    public static Order Create(string customerName, string email, ...) { ... }

    public void AddItem(MenuItem menuItem, int quantity)
    {
        if (!menuItem.IsAvailable) throw new OrderDomainException(...);
        // ...
        _items.Add(new OrderItem(...));
        RecalculateTotal();
    }
}
```

---

## 6. What “Done” Looks Like

- [ ] Application project has **zero** references to `Microsoft.EntityFrameworkCore` and Infrastructure.
- [ ] Domain entities contain meaningful behaviour; Value Objects are actually used.
- [ ] Every controller action goes exclusively through MediatR.
- [ ] `Review` has a complete Application vertical slice + API endpoint(s).
- [ ] Order creation is transactional (single save / proper UoW scope).
- [ ] Namespaces and folder structure are consistent under `Features/`.
- [ ] Architecture tests (or documented manual checklist) enforce the dependency rule.
- [ ] Existing happy-path flows (menu, reservation, order, Stripe checkout, auth) still work.
- [ ] SOLID violations listed in section 1 are resolved or explicitly documented as accepted debt with justification.

---

## 7. Out of Scope / Do Not Break
- Do not rewrite the entire frontend contract without necessity.
- Do not remove Stripe / Supabase optional integrations.
- Keep Docker / health / Serilog / rate-limiting behaviour intact.
- Migrations: prefer additive new migrations; do not casually drop production data columns.
- Preserve bilingual (EN/AR) readiness where it already exists in DTOs/controllers.

---

## 8. Agent Working Instructions

1. Start by reading the current code under `api-server/src` (especially Domain entities, Application Features, Infrastructure Repositories/UnitOfWork, and all Controllers).
2. Apply changes incrementally and keep the solution building after each phase.
3. Prefer small, reviewable commits conceptually: “stop EF leak”, “enrich Order aggregate”, “add Review feature”, etc.
4. After structural changes, run existing tests under `api-server/tests` and fix breakages.
5. Update or replace the existing `docs/backend/ARCHITECTURE_AUDIT_DEFECTS.md` and roadmap so they reflect the **new** reality (Auth is fixed; remaining defects are the ones above).
6. Produce a short final summary of files changed and remaining known debt.

**Primary goal:** Make this backend a **textbook Clean Architecture + SOLID** example that is maintainable, testable, and domain-centric — not just a folder layout that looks clean while still leaking EF and living with anemic entities.

Execute the plan thoroughly. Do not stop at cosmetic renames.
# Agent Prompt: Deep Clean Architecture & SOLID Hardening for Dawar Kitchen Backend

**Target:** `api-server/src` (NaarNoor.* projects)  
**Repo:** https://github.com/Mostafa-SAID7/Dawar-Kitchen-Full  
**Focus:** Make the backend **solid, clean, and deeply layered** — Domain-centric, CQRS + MediatR, true Clean Architecture, full SOLID compliance.  
**Date context:** Post-refactor (Auth moved to Application via MediatR). Audit defects partially addressed; many structural issues remain.

---

## 1. Current Architecture Snapshot (What Exists)

```
api-server/src/
├── NaarNoor.API/                 # Presentation (Controllers, Middleware, Configuration)
├── NaarNoor.Application/         # Use cases (CQRS Features, DTOs, Interfaces, Behaviours)
├── NaarNoor.Domain/              # Entities, Enums, ValueObjects, BaseEntity
└── NaarNoor.Infrastructure/      # EF Core, Repositories, External services (Stripe, Supabase, JWT, User)
```

**Good foundations already present:**
- Project separation matches Clean Architecture layers.
- MediatR + FluentValidation pipeline (`ValidationBehaviour`).
- Feature folders under Application (`Features/{Feature}/{Commands|Queries}/...`).
- Generic `IRepository<T>` + `IUnitOfWork` + `IApplicationDbContext`.
- Some domain behaviour on `Reservation` (state machine `TransitionTo`, `IsInValidState`).
- Value objects `Money` and `TimeSlot` exist (but are **unused**).
- Server-side price re-computation in order creation (security positive).
- Recent refactor moved Auth to Application layer and consolidated some service interfaces.

**Critical remaining problems (must fix):**

| # | Issue | Severity | Location / Symptom |
|---|-------|----------|--------------------|
| 1 | **Anemic Domain Model** | Critical | Entities are mostly POCOs with public setters. `Money`/`TimeSlot` never used. Business rules live in handlers or services. |
| 2 | **EF Core leak into Application** | Critical | Handlers import `Microsoft.EntityFrameworkCore` and call `.ToListAsync()`, `.Where()`, `.Query()`. Application depends on persistence technology. |
| 3 | **IQueryable exposure** | High | `IRepository.Query()` returns `IQueryable<T>` → Application can build arbitrary queries and is coupled to EF. |
| 4 | **Dual persistence abstraction** | High | Both `IUnitOfWork` **and** `IApplicationDbContext` used side-by-side (e.g. Stripe handlers inject both). Inconsistent. |
| 5 | **Incomplete vertical slices** | High | `Review` entity exists + migration, but **no** Application Features (no Commands/Queries/Handlers). No ReviewsController. |
| 6 | **Unit of Work incomplete** | Medium | `IUnitOfWork` missing `Reviews` (and possibly `Users`). Lazy repo fields only for a subset. |
| 7 | **Transaction boundaries weak** | Medium | `CreateOrder` does two separate `SaveChangesAsync` calls (Order then OrderItems) → risk of partial persist. |
| 8 | **Namespace inconsistency** | Medium | Some Features use `NaarNoor.Application.Orders...`, others `NaarNoor.Application.Features.Orders...`. |
| 9 | **Service interfaces vs Domain** | Medium | Auth/JWT/Stripe live as Application.Services interfaces implemented in Infrastructure — acceptable, but domain rules should not be pushed into them. |
| 10 | **No domain events / rich aggregates** | Medium | Order + OrderItems is a natural aggregate but not modelled as one. No domain events for OrderCreated, ReservationConfirmed, etc. |
| 11 | **Controller request DTOs mixed** | Low-Medium | Some request classes live inside controller files; mapping is ad-hoc. Prefer Application DTOs or dedicated API contracts. |
| 12 | **CacheService location** | Low | Lives under Application but uses DistributedCache — fine if abstracted; ensure no Infrastructure types leak. |

---

## 2. Target Architecture (What You Must Achieve)

### Dependency Rule (strict)
```
API  →  Application  →  Domain
         ↑
Infrastructure implements Application interfaces and depends on Domain
```
- **Never** let API or Application reference Infrastructure types/namespaces.
- Application must **not** reference `Microsoft.EntityFrameworkCore` (except possibly in tests).
- Domain must have **zero** dependencies on other projects or external frameworks.

### Folder & Layer Responsibilities

**Domain (`NaarNoor.Domain`)**
- Entities with **behaviour** (methods that enforce invariants).
- Value Objects (`Money`, `TimeSlot`, maybe `Email`, `PhoneNumber`, `Address`).
- Enums, Domain Exceptions, Domain Events (optional but recommended).
- Aggregates (e.g. `Order` owns `OrderItem` collection; factory methods).
- No DTOs, no EF attributes, no MediatR.

**Application (`NaarNoor.Application`)**
- Feature folders only: `Features/{Name}/{Commands|Queries}/{UseCaseName}/`.
- Commands / Queries / Handlers / Validators.
- Application interfaces: `IRepository` (or better specific read/write ports), `IUnitOfWork`, service ports (`IJwtService`, `IStripeService`, …).
- DTOs / Result models used by handlers.
- Pipeline behaviours (Validation, Logging, Transaction, Caching).
- **No** EF Core, no concrete DbContext, no Infrastructure namespaces.

**Infrastructure (`NaarNoor.Infrastructure`)**
- `ApplicationDbContext`, configurations, migrations, seeders.
- Concrete `Repository` / `UnitOfWork` implementations.
- External service implementations (Stripe, Supabase, JWT, UserService).
- DI registration only.

**API (`NaarNoor.API`)**
- Thin controllers that only map HTTP → MediatR command/query and map result → HTTP response.
- Middleware, configuration, Swagger, health.
- Request/response contracts can stay here **or** be thin wrappers around Application DTOs.

---

## 3. SOLID Principles — Concrete Enforcement Rules

### Single Responsibility (SRP)
- One handler = one use case.
- Controllers do **only** HTTP concerns (status codes, routing, auth attributes).
- Domain entity methods only enforce that entity’s invariants.
- Do not put reporting calculations, revenue aggregation, or Stripe session building inside controllers or fat services.

### Open/Closed (OCP)
- Prefer extending via new Commands/Queries/Behaviours rather than modifying existing handlers.
- Use strategy or domain events for side-effects (email, realtime) instead of hard-coding inside handlers.

### Liskov Substitution (LSP)
- Any implementation of `IRepository<T>` or service interfaces must be substitutable without changing handler behaviour.
- Avoid “optional” methods that throw `NotImplementedException`.

### Interface Segregation (ISP)
- Prefer small, focused interfaces over a god `IUnitOfWork` that grows forever.
- Consider splitting read vs write (CQRS-friendly): `IOrderReadRepository`, `IOrderWriteRepository` **or** keep UoW but make it complete and thin.
- Do not force handlers to depend on `IApplicationDbContext` when they only need a few repositories.

### Dependency Inversion (DIP)
- Application defines interfaces; Infrastructure implements them.
- Handlers depend only on abstractions (`IUnitOfWork`, `IStripeService`, etc.).
- **Eliminate** direct `IApplicationDbContext` injection in handlers where possible; route everything through repository/UoW abstractions.

---

## 4. Critical Fixes — Ordered Work Plan for the Agent

Execute in this order. Do not skip layers.

### Phase 0 — Inventory & Consistency (short)
1. Standardise namespaces: everything under `NaarNoor.Application.Features.{Feature}.{Commands|Queries}.{Name}`.
2. Confirm Auth is fully MediatR-driven (already done in latest code) — remove any residual direct Infrastructure calls from API.
3. Delete or archive outdated audit claims that no longer apply (Auth layer bypass is fixed).

### Phase 1 — Stop EF Leak (Critical)
1. **Change `IRepository<T>`** so it no longer exposes raw `IQueryable<T>` to Application, **or** introduce a clear boundary:
   - Preferred: keep `Query()` only if you also introduce specification/pattern or explicit methods (`GetAvailableMenuItemsAsync`, `GetByIdsAsync`, etc.).
   - Better long-term: replace free `IQueryable` with explicit query methods or a specification interface that Infrastructure translates.
2. **Remove all `using Microsoft.EntityFrameworkCore;` from Application project.**
   - Move any `.ToListAsync()` / `.FirstOrDefaultAsync()` into Infrastructure or behind repository methods that return `Task<List<T>>` / `Task<T?>`.
3. Make `IUnitOfWork` the **single** persistence entry point for write use-cases.
4. Either:
   - Expand `IUnitOfWork` to include `IRepository<Review> Reviews` (and `Users` if needed), **or**
   - Introduce dedicated repositories per aggregate and drop the dual `IApplicationDbContext` usage in handlers.
5. Fix `CreateOrderCommandHandler` (and similar) to use **one** `SaveChangesAsync` inside a proper transaction scope (or rely on a single unit-of-work save after building the full aggregate).

### Phase 2 — Enrich the Domain (Critical for “strong structure”)
1. **Use the existing Value Objects:**
   - Change `MenuItem.Price`, `Order.TotalAmount`, `OrderItem.UnitPrice` to use `Money` (or keep decimal for simplicity but wrap calculations with `Money` where currency matters).
   - Change Reservation time modelling to use `TimeSlot` where overlapping checks are needed.
2. Make entities richer:
   - `Order` becomes an aggregate root:
     - Private `List<OrderItem> _items` + public read-only collection.
     - Factory `Order.Create(...)` that validates customer data, type, and items.
     - Method `AddItem(MenuItem, quantity)` that uses server price and updates total.
     - Method `MarkPaid(string stripeSessionId)`, `Cancel()`, `TransitionStatus(...)` with state machine similar to Reservation.
   - `Reservation` already has good behaviour — keep and expand if needed (overlap checks via `TimeSlot`).
   - `MenuItem` — methods `MarkUnavailable()`, `UpdatePrice(Money)`, etc.
3. Add domain exceptions (`OrderDomainException`, `ReservationDomainException`) instead of generic `InvalidOperationException` for business rule violations.
4. (Recommended) Introduce simple domain events (`OrderCreatedEvent`, `ReservationConfirmedEvent`) and a MediatR notification or outbox later.

### Phase 3 — Complete Vertical Slices
1. **Review feature** (entity already exists + migration `20260913000000_AddReviewEntity`):
   - `Features/Reviews/Commands/CreateReview/`
   - `Features/Reviews/Queries/GetApprovedReviews/`
   - `Features/Reviews/Commands/ApproveReview/` (admin)
   - Wire controller + validation.
2. Ensure every public controller endpoint has a corresponding Command or Query (no business logic left in controllers).
3. Add missing validators for every command that accepts user input.

### Phase 4 — Repository & Unit-of-Work Hardening
1. Make `UnitOfWork` complete (all aggregates).
2. Prefer constructor injection of repositories over lazy `??=` fields if it improves testability.
3. Consider adding a transactional behaviour or explicit `IUnitOfWork.BeginTransactionAsync` if multi-aggregate consistency is required.
4. Keep EF configurations and migrations only in Infrastructure.

### Phase 5 — API Layer Cleanup
1. Controllers stay thin: create command/query → `_mediator.Send` → map to `IActionResult`.
2. Move any remaining request classes that contain business rules into Application.
3. Ensure consistent status codes and ProblemDetails / exception middleware mapping for domain vs validation exceptions.
4. Keep Swagger and health endpoints as-is (already good).

### Phase 6 — Testing & Guardrails
1. Domain unit tests for aggregate behaviour and state machines (no EF).
2. Application handler tests with mocked `IUnitOfWork` / service interfaces (no real DB).
3. Infrastructure integration tests against Testcontainers or in-memory only where necessary.
4. Add an architecture test (e.g. NetArchTest or custom) that fails if:
   - Application references Infrastructure or EF Core.
   - Domain references anything outside itself.
   - API references Infrastructure (except DI composition root).

---

## 5. Concrete Code Patterns the Agent Must Follow

### Handler shape (after fixes)
```csharp
// Application — NO Microsoft.EntityFrameworkCore
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IUnitOfWork _uow;
    // only Application abstractions

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var menuItems = await _uow.MenuItems.GetAvailableByIdsAsync(
            request.Items.Select(i => i.MenuItemId), ct);

        // domain logic
        var order = Order.Create(...); // factory
        foreach (var item in ...)
            order.AddItem(...);

        _uow.Orders.Add(order);
        await _uow.SaveChangesAsync(ct); // single save
        return order.Id;
    }
}
```

### Repository contract direction
```csharp
// Prefer explicit methods over free IQueryable for Application consumers
public interface IMenuItemRepository
{
    Task<IReadOnlyList<MenuItem>> GetAvailableByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct);
    // ...
}
```
(You may keep a generic repository for simple CRUD, but complex queries belong behind named methods or specifications implemented in Infrastructure.)

### Domain aggregate sketch
```csharp
public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Money Total { get; private set; }

    public static Order Create(string customerName, string email, ...) { ... }

    public void AddItem(MenuItem menuItem, int quantity)
    {
        if (!menuItem.IsAvailable) throw new OrderDomainException(...);
        // ...
        _items.Add(new OrderItem(...));
        RecalculateTotal();
    }
}
```

---

## 6. What “Done” Looks Like

- [ ] Application project has **zero** references to `Microsoft.EntityFrameworkCore` and Infrastructure.
- [ ] Domain entities contain meaningful behaviour; Value Objects are actually used.
- [ ] Every controller action goes exclusively through MediatR.
- [ ] `Review` has a complete Application vertical slice + API endpoint(s).
- [ ] Order creation is transactional (single save / proper UoW scope).
- [ ] Namespaces and folder structure are consistent under `Features/`.
- [ ] Architecture tests (or documented manual checklist) enforce the dependency rule.
- [ ] Existing happy-path flows (menu, reservation, order, Stripe checkout, auth) still work.
- [ ] SOLID violations listed in section 1 are resolved or explicitly documented as accepted debt with justification.

---

## 7. Out of Scope / Do Not Break
- Do not rewrite the entire frontend contract without necessity.
- Do not remove Stripe / Supabase optional integrations.
- Keep Docker / health / Serilog / rate-limiting behaviour intact.
- Migrations: prefer additive new migrations; do not casually drop production data columns.
- Preserve bilingual (EN/AR) readiness where it already exists in DTOs/controllers.

---

## 8. Agent Working Instructions

1. Start by reading the current code under `api-server/src` (especially Domain entities, Application Features, Infrastructure Repositories/UnitOfWork, and all Controllers).
2. Apply changes incrementally and keep the solution building after each phase.
3. Prefer small, reviewable commits conceptually: “stop EF leak”, “enrich Order aggregate”, “add Review feature”, etc.
4. After structural changes, run existing tests under `api-server/tests` and fix breakages.
5. Update or replace the existing `docs/backend/ARCHITECTURE_AUDIT_DEFECTS.md` and roadmap so they reflect the **new** reality (Auth is fixed; remaining defects are the ones above).
6. Produce a short final summary of files changed and remaining known debt.

**Primary goal:** Make this backend a **textbook Clean Architecture + SOLID** example that is maintainable, testable, and domain-centric — not just a folder layout that looks clean while still leaking EF and living with anemic entities.

Execute the plan thoroughly. Do not stop at cosmetic renames.
