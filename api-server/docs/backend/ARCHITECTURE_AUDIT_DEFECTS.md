# Backend Architecture Audit: Current Defects & Debt
**Date:** September 13, 2026 (Post-Refactor)  
**Scope:** Clean Architecture layer violations, transaction safety, domain richness  
**Status:** 11 DEFECTS IDENTIFIED (4 CRITICAL, 7 HIGH/MEDIUM)

---

## Executive Summary

After recent refactoring (Auth moved to Application via MediatR, DTOs consolidated, service interfaces organized), the backend has **improved significantly** but retains critical architectural issues:

| # | Defect | Severity | Impact | Status |
|---|--------|----------|--------|--------|
| 1 | Transaction safety — multiple SaveChangesAsync calls | **CRITICAL** | Data consistency risk in Order & Stripe handlers | Needs fix |
| 2 | EF Core leak — `.ToListAsync()` in 14 handlers | **CRITICAL** | Application tightly coupled to EF; DIP violated | Needs architectural fix |
| 3 | IRepository exposes raw IQueryable<T> | **CRITICAL** | Application can build arbitrary queries; hard to test | Design issue |
| 4 | IUnitOfWork missing User & Review repositories | **HIGH** | Incomplete abstraction; Review feature orphaned | Needs completion |
| 5 | Dual persistence abstraction (UoW + DbContext) | **HIGH** | CreateStripeCheckoutSessionCommandHandler bypasses UoW | Inconsistent pattern |
| 6 | Review entity orphaned (no Application features) | **HIGH** | Entity exists in DB; no way to CRUD via API | Incomplete slice |
| 7 | Order aggregate not modelled as aggregate | **MEDIUM** | Order+OrderItems not treated as cohesive unit; multiple saves | Design debt |
| 8 | Value Objects unused (Money, TimeSlot) | **MEDIUM** | Defined but not leveraged; domain model anemic | Design debt |
| 9 | No domain exceptions (using generic InvalidOperationException) | **MEDIUM** | Business rule violations not clearly signalled | Poor semantics |
| 10 | No domain events | **MEDIUM** | Cannot decouple side-effects (email, notifications); state changes invisible | Future scalability issue |
| 11 | Controller business logic (ChefsController, HealthController) | **MEDIUM** | Separation of concerns violated; logic should be in handlers | Partial refactor |

---

## What's Fixed ✅

- ✅ **Auth Layer:** Now fully MediatR-driven (RegisterUserCommand, LoginUserQuery)
- ✅ **DTOs Consolidated:** All DTOs in Application/DTOs/ (AuthDtos, MenuItemDto, etc.)
- ✅ **Service Interfaces:** Centralized in Application/Services/ (IJwtService, IStripeService, etc.)
- ✅ **Feature Organization:** All features under Features/{Name}/{Commands|Queries}/ subfolders
- ✅ **No Infrastructure Duplication:** All interfaces now in Application layer only
- ✅ **API Layer:** Most controllers properly dispatch via MediatR (Auth, Menu, Orders, Payments, Contact, Reservations, Chefs)

---

## Critical Defects — Detailed Analysis

### Defect #1: Transaction Safety — Multiple SaveChangesAsync Calls ⚠️ CRITICAL

**Location & Code:**

**CreateOrderCommandHandler** (lines 53 and 75 — **TWO separate SaveChangesAsync**)
```csharp
public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
{
    // ... validation ...
    var order = new Order { ... };
    
    _unitOfWork.Orders.Add(order);
    await _unitOfWork.SaveChangesAsync(cancellationToken);  // ← FIRST SAVE
    
    // ... build order items ...
    foreach (var (requestItem, menuItem, serverPrice) in validatedItems)
    {
        var orderItem = new OrderItem { ... };
        _unitOfWork.OrderItems.Add(orderItem);
    }
    
    await _unitOfWork.SaveChangesAsync(cancellationToken);  // ← SECOND SAVE
    
    return order.Id;
}
```

**CreateStripeCheckoutSessionCommandHandler** (lines 74 and 96 — **TWO separate SaveChangesAsync**)
```csharp
public async Task<CreateStripeCheckoutSessionResponse> Handle(...)
{
    // ... build order with items ...
    var order = new Order { Items = items, ... };
    
    _unitOfWork.Orders.Add(order);
    await _unitOfWork.SaveChangesAsync(cancellationToken);  // ← FIRST SAVE
    
    // ... create Stripe session ...
    var checkoutResult = await _stripe.CreateCheckoutSessionAsync(...);
    
    order.StripeSessionId = checkoutResult.SessionId;
    await _unitOfWork.SaveChangesAsync(cancellationToken);  // ← SECOND SAVE
}
```

**Impact:**
- **Order created but items lost:** If second SaveChangesAsync fails, orphaned Order persists without OrderItems
- **Stripe session lost:** If second save fails in Stripe handler, Order has no session ID (Stripe reconciliation broken)
- **Race condition:** Between saves, another request could query incomplete order

**Fix Required:** Consolidate to **single SaveChangesAsync** within transaction scope or use proper DbTransaction wrapper.

---

### Defect #2: EF Core Leak — 14 Handlers Import Microsoft.EntityFrameworkCore ⚠️ CRITICAL

**Files & Imports:**
```
GetReservationsQueryHandler.cs:2         — using Microsoft.EntityFrameworkCore
GetReservationByIdQueryHandler.cs:2      — using Microsoft.EntityFrameworkCore
UpdateReservationCommandHandler.cs:2     — using Microsoft.EntityFrameworkCore
DeleteReservationCommandHandler.cs:2     — using Microsoft.EntityFrameworkCore
HandleStripeWebhookCommandHandler.cs:2   — using Microsoft.EntityFrameworkCore
CreateStripeCheckoutSessionCommandHandler.cs:2  — using Microsoft.EntityFrameworkCore
UpdateMenuItemCommandHandler.cs:2        — using Microsoft.EntityFrameworkCore
GetMenuItemsQueryHandler.cs:2            — using Microsoft.EntityFrameworkCore
GetMenuItemsCachedQueryHandler.cs:2      — using Microsoft.EntityFrameworkCore
DeleteMenuItemCommandHandler.cs:2        — using Microsoft.EntityFrameworkCore
GetMenuItemByIdQueryHandler.cs:2         — using Microsoft.EntityFrameworkCore
GetChefsQueryHandler.cs:2                — using Microsoft.EntityFrameworkCore
CreateOrderCommandHandler.cs:2           — using Microsoft.EntityFrameworkCore
```

**Methods Used (EF Core specific):**
- `.ToListAsync()` (14 files)
- `.FirstOrDefaultAsync()` (6 files)
- `.Where()` (chain on IQueryable, not LINQ-to-Objects)
- `.Select()` (LINQ projection, not available for `IEnumerable`)
- `.CountAsync()` (EF Core async)
- `.ToDictionaryAsync()` (EF Core async)

**Impact:**
- **Dependency Inversion Principle (DIP) violated:** Application depends on concrete infrastructure (EF Core)
- **Testability:** Cannot test handlers with in-memory collections; must mock entire DbSet/IQueryable behavior
- **Portability:** Switching database providers (e.g., EF Core → Dapper) requires rewriting all 14 handlers
- **Hidden coupling:** Application logic is implicitly coupled to EF Core's LINQ provider semantics

**Fix Required:** Move all EF Core operations into Infrastructure (Repository methods), have Application receive `Task<List<T>>` or `Task<T?>`, not `IQueryable<T>`.

---

### Defect #3: IRepository Exposes Raw IQueryable<T> ⚠️ CRITICAL

**Location:** `Application/Common/Interfaces/IRepository.cs`

```csharp
public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();  // ← EXPOSES raw LINQ provider
    
    Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    void Add(TEntity entity);
    void Remove(TEntity entity);
    void Update(TEntity entity);
}
```

**Why This Violates SOLID:**
- **Interface Segregation (ISP) violated:** Forces all consumers to depend on LINQ composition, even if they don't need it
- **Dependency Inversion (DIP) violated:** Application becomes aware of `IQueryable` (EF Core abstraction)
- **Open/Closed (OCP) violated:** Adding new query patterns requires modifying handlers; can't extend in Infrastructure

**Alternative Pattern (Better):**
```csharp
// Option 1: Explicit query methods (recommended)
public interface IMenuItemRepository : IRepository<MenuItem>
{
    Task<IReadOnlyList<MenuItem>> GetAvailableByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken ct);
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct);
    // ... more explicit methods ...
}

// Option 2: Keep Query() but return Task<List<T>>
public interface IRepository<TEntity> where TEntity : class
{
    // Execute in Infrastructure, return complete collection
    Task<List<TEntity>> QueryAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? filter = null,
        CancellationToken ct = default);
}
```

**Fix Required:** Remove `IQueryable<T> Query()` OR restrict it to Infrastructure only.

---

### Defect #4: IUnitOfWork Missing Repositories ⚠️ HIGH

**Location:** `Application/Common/Interfaces/IUnitOfWork.cs`

```csharp
public interface IUnitOfWork
{
    IRepository<Reservation> Reservations { get; }
    IRepository<MenuItem> MenuItems { get; }
    IRepository<Chef> Chefs { get; }
    IRepository<ContactInquiry> ContactInquiries { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    // ❌ MISSING: User repository
    // ❌ MISSING: Review repository
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

**Related Issue:** `IApplicationDbContext` has 8 DbSets, but UoW only exposes 6.

```csharp
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }                     // ❌ Not in UoW
    DbSet<Reservation> Reservations { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<Chef> Chefs { get; }
    DbSet<ContactInquiry> ContactInquiries { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Review> Reviews { get; }                 // ❌ Not in UoW
    
    Task<int> SaveChangesAsync(CancellationToken ct);
}
```

**Impact:**
- **HealthController** (line 16) must inject `IApplicationDbContext` directly to access Users: `_dbContext.Users.AnyAsync()`
- **Review feature** cannot be implemented via UoW — orphaned entity
- **Inconsistent abstraction:** Some aggregates accessed via UoW, others via DbContext directly

**Fix Required:** Add User and Review repositories to IUnitOfWork.

---

### Defect #5: Dual Persistence Abstraction ⚠️ HIGH

**Dual Injection Pattern:**

**CreateStripeCheckoutSessionCommandHandler** (lines 12-18):
```csharp
public CreateStripeCheckoutSessionCommandHandler(
    IUnitOfWork unitOfWork,           // ← Abstraction
    IApplicationDbContext db,         // ← Concrete implementation leak!
    IStripeService stripe)
{
    _unitOfWork = unitOfWork;
    _db = db;
    _stripe = stripe;
}
```

**Usage (lines 34-44):**
```csharp
var menuItems = await _db.MenuItems        // ← Uses DbContext directly
    .Where(m => requestedIds.Contains(m.Id) && m.IsAvailable)
    .ToDictionaryAsync(m => m.Id, cancellationToken);
```

**Impact:**
- **Inconsistency:** Other handlers use `_unitOfWork.MenuItems.Query()`, this one uses `_db.MenuItems`
- **Abstraction leak:** DbContext internals exposed to Application layer
- **Hard to mock:** Unit tests must mock both UoW and DbContext
- **Hidden transaction scope:** Not clear if operations are within same transaction

**Fix Required:** Use **only** IUnitOfWork throughout; move DbContext injection only to Infrastructure.

---

### Defect #6: Review Feature Orphaned ⚠️ HIGH

**Exists in Domain:**
- `NaarNoor.Domain/Entities/Review.cs` ✅
- Migration `20260913000000_AddReviewEntity` ✅
- `Infrastructure/Configurations/ReviewConfiguration.cs` ✅

**Missing in Application:**
- ❌ `Application/Features/Reviews/Commands/` (no CreateReview, UpdateReview, etc.)
- ❌ `Application/Features/Reviews/Queries/` (no GetReviews, GetReviewById, etc.)
- ❌ `Application/DTOs/ReviewDto.cs`

**Missing in API:**
- ❌ `API/Controllers/ReviewsController.cs`

**Impact:**
- Entity in database but **unreachable** via API
- Cannot create, read, list, or delete reviews
- Incomplete vertical slice
- Confusing codebase (why is Review in Domain if not implemented?)

**Fix Required:** Complete Review vertical slice with Commands, Queries, and Controller.

---

### Defect #7: Order Not Modelled as Aggregate ⚠️ MEDIUM

**Current State:**
```csharp
public class Order : BaseEntity
{
    public string CustomerName { get; set; } = "";
    public string Email { get; set; } = "";
    public int Quantity { get; set; }
    // ... 15+ properties, all public setters ...
    
    public List<OrderItem> Items { get; set; } = new();  // ← Collection, but not protected
}

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
    // ... properties ...
}
```

**Issue:** Order should be an **Aggregate Root** with encapsulated business logic:

**Current (Anemic):**
```csharp
var order = new Order { CustomerName = "...", Items = items, ... };  // ← Direct instantiation
_unitOfWork.Orders.Add(order);
await _unitOfWork.SaveChangesAsync();
```

**Desired (Rich Aggregate):**
```csharp
var order = Order.Create(customerName, email, orderType);  // ← Factory
foreach (var item in requestedItems)
    order.AddItem(menuItem, quantity);  // ← Behavior

order.MarkPaid(stripeSessionId);  // ← State transitions
_unitOfWork.Orders.Add(order);
await _unitOfWork.SaveChangesAsync();  // ← SINGLE save
```

**Impact:**
- No validation at aggregate boundary
- Items could be added/removed without recalculating totals
- State transitions (Pending→Paid→Shipped) unenforceable
- Multiple SaveChangesAsync calls needed (defect #1 root cause)

**Fix Required:** Introduce Order aggregate factory, AddItem behavior, state machine.

---

### Defect #8: Value Objects Unused ⚠️ MEDIUM

**Defined but Dormant:**

**Money** (ValueObjects/Money.cs):
```csharp
public class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }  // Validates ISO 4217
    // Equals, GetHashCode, ToString implemented
}
```

**Used?** NO — All prices in Order, OrderItem, MenuItem stored as raw `decimal Price`.

**TimeSlot** (ValueObjects/TimeSlot.cs):
```csharp
public class TimeSlot : IEquatable<TimeSlot>
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    
    public bool Overlaps(TimeSlot other) { ... }
    public bool Contains(TimeOnly time) { ... }
}
```

**Used?** NO — Reservation uses separate `TimeOnly ReservationTime` + date validation elsewhere.

**Impact:**
- Valuable domain concepts exist but unused
- Price calculations use raw decimals (lose currency context)
- Reservation overlap checks not leveraging TimeSlot logic
- Domain model remains anemic despite infrastructure

**Fix Required:** Integrate Money into MenuItem.Price, Order.TotalAmount, OrderItem.UnitPrice; integrate TimeSlot into Reservation time-based queries.

---

### Defect #9: No Domain Exceptions ⚠️ MEDIUM

**Current Pattern:**
```csharp
if (status is not valid)
    throw new InvalidOperationException("Cannot transition from X to Y");  // ← Generic
    
if (!menuItem.IsAvailable)
    throw new InvalidOperationException("Menu item not available");  // ← Generic
```

**Problem:** Generic exceptions hide domain meaning. A caller cannot distinguish between:
- Invalid business rule (can retry after fixing data)
- Programming error (configuration missing)
- System error (database down)

**Better Pattern:**
```csharp
public class OrderDomainException : DomainException { }
public class ReservationDomainException : DomainException { }

// Usage:
if (status is not valid)
    throw new ReservationDomainException("Cannot transition from X to Y");
```

**Impact:**
- Exception handlers in API layer cannot distinguish domain errors from system errors
- No semantic clarity for callers
- Hard to write specific exception tests

**Fix Required:** Create domain exception hierarchy (OrderDomainException, ReservationDomainException, etc.).

---

### Defect #10: No Domain Events ⚠️ MEDIUM

**Current State:** Order created, Reservation confirmed, but no one notified.

**Problem Scenario:**
```
1. Order created
2. Email service should be triggered (welcome email + receipt)
3. Reservation confirmed
4. Real-time notification should be sent to admin dashboard
5. Review submitted
6. Notification to restaurant staff
```

**Current Implementation:** Email/notifications would need to be wired into handlers directly, creating tight coupling.

**Better Pattern:** Domain events:
```csharp
public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; init; }
    public string CustomerEmail { get; init; }
}

// In handler:
_uow.Orders.Add(order);
order.RaiseEvent(new OrderCreatedEvent { OrderId = order.Id, ... });
await _uow.SaveChangesAsync();

// Email service subscribes to event:
public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent @event, ...) { ... }
}
```

**Impact:**
- Side-effects (email, notifications, real-time) loosely coupled
- Can add/remove handlers without modifying Order or handler logic
- Future scalability: can publish to message queue

**Fix Required:** Introduce domain events (architectural debt, not immediate blocker).

---

### Defect #11: Controller Business Logic ⚠️ MEDIUM

**ChefsController.GetById** (lines 34-50):
```csharp
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
{
    var chef = await _unitOfWork.Chefs.Query()  // ← Query logic in controller
        .Where(c => c.Id == id && c.IsActive)
        .Select(c => new ChefDto { ... })
        .FirstOrDefaultAsync(cancellationToken);
    
    if (chef is null) return NotFound();
    return Ok(chef);
}
```

**Should be:**
```csharp
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
{
    var result = await _mediator.Send(new GetChefByIdQuery(id), cancellationToken);
    return result is null ? NotFound() : Ok(result);
}
```

**HealthController.GetHealth** (lines 16-41):
```csharp
public async Task<IActionResult> GetHealth()
{
    // Direct DbContext injection and database logic
    var canConnect = await _dbContext.Users.AnyAsync();
    // ... logic to build response ...
}
```

**Should be:**
```csharp
public async Task<IActionResult> GetHealth()
{
    var result = await _mediator.Send(new GetHealthQuery(), cancellationToken);
    return Ok(result);
}
```

**Impact:**
- Controllers have business logic (should only map HTTP → MediatR)
- Query building scattered across codebase
- Testing requires HTTP context
- Reusability compromised (cannot call logic from other contexts)

**Fix Required:** Move business logic to handlers; controllers dispatch only.

---

## Remaining Architecture Debt (Not Blocking)

| Item | Severity | Reason Not Blocking | Future Action |
|------|----------|-------------------|----------------|
| Generic Repository pattern | Medium | Works at current scale; not urgent | Introduce aggregate-specific repos when scaling |
| Missing IApplicationDbContext abstraction | Low | Works for health checks; acceptable | Can improve later |
| No pagination in list queries | Low | MVP doesn't require; works on small dataset | Add when dataset grows |
| No soft deletes or audit trail | Low | Business rules don't require; can add later | Extend BaseEntity when needed |

---

## Recommended Fix Priority (by risk & effort)

### Phase 1: Critical Fixes (do immediately)
| # | Fix | Effort | Risk |
|---|-----|--------|------|
| 1.1 | Consolidate transaction: CreateOrder & Stripe handlers to single SaveChangesAsync | 30 min | Low (contained fix) |
| 1.2 | Remove IApplicationDbContext injection; use IUnitOfWork only | 30 min | Low (few usages) |
| 1.3 | Add User & Review repositories to IUnitOfWork | 20 min | Low (additive) |

### Phase 2: Architectural Fixes (this session)
| # | Fix | Effort | Risk |
|---|-----|--------|------|
| 2.1 | Remove `IQueryable<T>` from IRepository; introduce explicit query methods | 2 hours | Medium (refactoring) |
| 2.2 | Move `.ToListAsync()` to Infrastructure; return Task<List<T>> to Application | 3 hours | Medium (systematic) |
| 2.3 | Complete Review feature vertical slice (Commands, Queries, Controller) | 1 hour | Low (new code) |
| 2.4 | Move ChefsController & HealthController query logic to handlers | 30 min | Low (contained fix) |

### Phase 3: Domain Enrichment (future)
| # | Fix | Effort | Risk |
|---|-----|--------|------|
| 3.1 | Introduce Order aggregate factory & AddItem behavior | 1 hour | Low (encapsulation) |
| 3.2 | Create domain exception hierarchy | 30 min | Low (new code) |
| 3.3 | Integrate Money value object into pricing | 1 hour | Medium (migration needed) |
| 3.4 | Integrate TimeSlot into reservation logic | 1 hour | Low (utility enhancement) |

### Phase 4: Future Scalability (not this session)
| # | Fix | Effort | Risk |
|---|-----|--------|------|
| 4.1 | Introduce domain events & outbox pattern | 4+ hours | High (new infrastructure) |
| 4.2 | Introduce aggregate-specific repositories | 4+ hours | High (systematic refactor) |
| 4.3 | Add specification pattern for complex queries | 3+ hours | Medium (new abstraction) |

---

## References

- Clean Architecture rule: API → Application → Infrastructure (DIP)
- SOLID: Dependency Inversion (depend on abstractions, not concretions)
- Transaction safety: Atomic operations within single SaveChangesAsync boundary
- Repository pattern: Encapsulate data access; avoid exposing LINQ providers
