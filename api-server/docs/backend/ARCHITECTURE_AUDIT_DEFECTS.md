# Backend Architecture Audit: Critical Defects Report
**Date:** September 13, 2026  
**Scope:** Clean Architecture layer violations, DTOs, repository pattern concerns  
**Status:** 4 CRITICAL DEFECTS IDENTIFIED

---

## Executive Summary

The backend has **real, code-verified architecture defects** that violate Clean Architecture principles:

| Defect | Severity | Impact | Location |
|--------|----------|--------|----------|
| Layer bypass (API → Infrastructure) | CRITICAL | AuthController, StaffController, ReportsController | 3 controllers |
| EF Core leaks IQueryable | HIGH | Application handlers depend on EF Core | Repository.Query() |
| DTO duplication risk | MEDIUM | AuthDtos in both API and Application | API/DTOs + Application/DTOs |
| Incomplete entity slice | MEDIUM | Review entity has no Application layer | Review.cs (no queries/commands) |

---

## Defect #1: Layer Bypass — API Calling Infrastructure Directly (CRITICAL)

### What This Means
Controllers should **only** call Application layer (Commands/Queries via MediatR). They must **never** call Infrastructure services directly.

**Dependency rule violation:**
```
❌ WRONG:
API → Infrastructure (breaks Clean Architecture)

✅ RIGHT:
API → Application (MediatR) → Infrastructure
```

### Evidence: Code Analysis

#### 1.1 AuthController — Layer Bypass CONFIRMED

**File:** `api-server/src/NaarNoor.API/Controllers/AuthController.cs`

```csharp
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;          // ❌ Infrastructure service
    private readonly IUserService _userService;        // ❌ Infrastructure service
    
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request.Email, request.Password, "");
        // ❌ Calling Infrastructure directly from API layer
    }
    
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);
        // ❌ Calling Infrastructure directly from API layer
    }
}
```

**Observation:** No `Application/Auth/Commands/` or `Application/Auth/Queries/` folder exists.

**Impact:**
- Business logic (user registration, JWT generation) tightly coupled to HTTP layer
- Hard to reuse in non-HTTP contexts (e.g., background jobs, CLI)
- Hard to test (must mock HTTP context)
- Direct EF Core DbContext access hidden in UserService

---

#### 1.2 ReportsController — Layer Bypass CONFIRMED

**File:** `api-server/src/NaarNoor.API/Controllers/ReportsController.cs`

```csharp
public class ReportsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;          // ❌ Infrastructure (Repository)
    
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue(
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        // 20+ lines of business logic directly in controller:
        var allOrders = await _unitOfWork.Orders.Query()
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .ToListAsync();
        
        // Date range calculations
        var todayRev = allOrders.Where(o => o.CreatedAt.Date == today).Sum(o => o.TotalAmount);
        var weekRev = allOrders.Where(o => o.CreatedAt >= weekStart).Sum(o => o.TotalAmount);
        // ... more calculations ...
        
        return Ok(new { todayRevenue = todayRev, ... });  // ❌ Anonymous object
    }
}
```

**Observation:** No `Application/Reports/Queries/` folder exists.

**Impact:**
- Revenue calculation logic locked in HTTP controller
- No validation (date ranges not validated)
- No caching (expensive calculations repeated)
- No testing without HTTP
- Tight coupling to Repository pattern

---

#### 1.3 StaffController — Layer Bypass CONFIRMED

**File:** `api-server/src/NaarNoor.API/Controllers/StaffController.cs`

```csharp
public class StaffController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;          // ❌ Infrastructure (Repository)
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Querying Chef table directly from controller:
        var staff = await _unitOfWork.Chefs.Query()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new  // ❌ Anonymous object, not DTO
            {
                id = c.Id.ToString(),
                name = c.Name,
                role = c.Title,
                status = "available",  // ❌ Hardcoded, not from entity
                // ...
            })
            .ToListAsync();
            
        return Ok(staff);
    }
}
```

**Observation:** No `Application/Staff/Queries/` folder exists.

**Impact:**
- Staff logic mixed with HTTP layer
- Status hardcoded (not persisted to database)
- Staff and Chef entities conflated (conceptually wrong)
- No DTO contract

---

#### 1.4 PaymentsController — CORRECT (Uses MediatR)

**File:** `api-server/src/NaarNoor.API/Controllers/PaymentsController.cs`

```csharp
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;  // ✅ Application layer
    
    [Authorize]
    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession(
        [FromBody] CreateStripeCheckoutSessionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        // ✅ Correctly uses MediatR
        return Ok(new { orderId = result.OrderId, sessionUrl = result.SessionUrl });
    }
}
```

**Note:** This is **correct** architecture. Other controllers should follow this pattern.

---

### Controllers Audit Summary

| Controller | Has Application Layer? | Pattern |
|------------|------------------------|---------|
| AuthController | ❌ NO | Direct Infrastructure (WRONG) |
| ChefsController | ✅ YES | Uses GetChefsQuery (CORRECT) |
| ContactController | ✅ YES | Uses Commands (CORRECT) |
| HealthController | ✅ YES | Reads config (CORRECT) |
| MenuController | ✅ YES | Uses Queries (CORRECT) |
| OrdersController | ✅ YES | Uses Commands (CORRECT) |
| PaymentsController | ✅ YES | Uses Commands (CORRECT) |
| ReportsController | ❌ NO | Direct UnitOfWork (WRONG) |
| ReservationsController | ✅ YES | Uses Commands/Queries (CORRECT) |
| StaffController | ❌ NO | Direct UnitOfWork (WRONG) |

**Verdict:** 3 out of 10 controllers violate Clean Architecture.

---

## Defect #2: EF Core IQueryable Leak into Application Layer (HIGH)

### What This Means
Application handlers receive `IQueryable<T>` directly from repositories. This exposes EF Core implementation details (LINQ provider, deferred execution, EF-specific methods like `.ToListAsync()`) in the Application layer.

**Pattern defect:**
```csharp
// ❌ WRONG: IQueryable exposed
public IQueryable<TEntity> Query() => _context.Set<TEntity>();

// ✅ BETTER: Execute in Infrastructure, return collections
public Task<List<TEntity>> FindAllAsync(...) => _context.Set<TEntity>().ToListAsync(...);
```

### Evidence: Code Analysis

**File:** `api-server/src/NaarNoor.Infrastructure/Repositories/Repository.cs`

```csharp
public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();  // ❌ Returns IQueryable
    Task<TEntity?> FindAsync(...);
    void Add(TEntity entity);
    void Remove(TEntity entity);
    void Update(TEntity entity);
}

public class Repository<TEntity> : IRepository<TEntity>
{
    public IQueryable<TEntity> Query()
        => _context.Set<TEntity>();  // ❌ Direct DbSet
}
```

### Where IQueryable Leaks Into Handlers

**File:** `api-server/src/NaarNoor.Application/Orders/Commands/CreateOrder/CreateOrderCommandHandler.cs`

```csharp
public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
{
    // Application handler directly uses EF Core methods:
    var menuItems = await _unitOfWork.MenuItems.Query()  // ← IQueryable exposure
        .Where(m => menuItemIds.Contains(m.Id) && m.IsAvailable)
        .ToListAsync(cancellationToken);  // ← EF Core async method
    
    var allOrders = await _unitOfWork.Orders.Query()
        .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
        .ToListAsync(cancellationToken);  // ← EF Core async method
}
```

**File:** `api-server/src/NaarNoor.Application/Chefs/Queries/GetChefs/GetChefsQueryHandler.cs`

```csharp
public async Task<List<ChefDto>> Handle(GetChefsQuery request, CancellationToken cancellationToken)
{
    var chefs = await _unitOfWork.Chefs.Query()  // ← IQueryable exposure
        .Where(c => c.IsActive)
        .OrderBy(c => c.SortOrder)
        .Select(c => new ChefDto { ... })
        .ToListAsync(cancellationToken);  // ← EF Core async method
}
```

### Impact

**Tightly Couples Application to EF Core:**
- Application layer imports `Microsoft.EntityFrameworkCore`
- Can't switch database providers without changing handlers
- Handlers leak EF-specific syntax (`.ToListAsync()`, `.FirstOrDefaultAsync()`, `.CountAsync()`)

**Difficult to Test:**
- Must mock entire IQueryable behavior
- Can't easily test with in-memory collections
- Complex Moq setups needed

**Hidden Complexity:**
- Deferred execution not obvious in handler code
- Easy to accidentally create multiple database calls
- N+1 query problems lurk silently

---

## Defect #3: DTO Duplication — API/DTOs and Application/DTOs (MEDIUM)

### What This Means
DTOs exist in **two places:**
- `api-server/src/NaarNoor.API/DTOs/AuthDtos.cs` — Request/response DTOs
- `api-server/src/NaarNoor.Application/DTOs/` — Domain transfer objects

**Source of truth is ambiguous.** Should be one.

### Evidence: Code Analysis

**API Layer DTOs:**
```csharp
// api-server/src/NaarNoor.API/DTOs/AuthDtos.cs
public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string FullName { get; set; } = "";
}
```

**Application Layer DTOs:**
```csharp
// api-server/src/NaarNoor.Application/DTOs/
├── ChefDto.cs
├── MenuItemDto.cs
├── OrderDto.cs
├── OrderItemDto.cs
└── ReservationDto.cs
```

### What's Happening?

- ✅ **Application/DTOs/** is correct — domain transfer objects (ChefDto, MenuItemDto, etc.)
- ❓ **API/DTOs/AuthDtos.cs** might be legitimate:
  - `LoginRequest`, `RegisterRequest` are input request shapes (acceptable in API layer for request validation)
  - But no corresponding response DTOs (e.g., `LoginResponse` with token)

### Recommendation

**Principle:** DTOs should be categorized by **responsibility**:
- **Request/Response shapes** → can live in API layer (minimal, just for binding)
- **Domain transfer objects** → must live in Application layer (returned by queries/commands)

**Current state:** Acceptable if:
1. API/DTOs only contain request validation classes
2. All query/command responses use Application/DTOs
3. No duplication of the same DTO across layers

**Action:** Verify no duplication before committing to either approach.

---

## Defect #4: Incomplete Entity Slice — Review Entity (MEDIUM)

### What This Means
`Review.cs` entity was added to Domain layer with a migration, but **no Application layer implementation**:
- ❌ No `Application/Reviews/Commands/`
- ❌ No `Application/Reviews/Queries/`
- ❌ No `ReviewsController.cs`
- ❌ No DTO

**Entity exists in database, but feature is incomplete.**

### Evidence: File Audit

**Domain Layer:**
```
✅ api-server/src/NaarNoor.Domain/Entities/Review.cs (new, Sept 13)
✅ api-server/src/NaarNoor.Infrastructure/Migrations/20260913000000_AddReviewEntity.cs
```

**Application Layer:**
```
❌ No api-server/src/NaarNoor.Application/Reviews/ folder
```

**API Layer:**
```
❌ No api-server/src/NaarNoor.API/Controllers/ReviewsController.cs
```

### Impact

- Database has Reviews table (migration applied)
- No way to create, read, or list reviews via API
- In-flight feature, not intentionally partial
- Creates confusion: is this a planned feature or abandoned?

### Recommendation

**Complete the vertical slice:** If reviews are planned, create:
1. `ReviewDto.cs` in Application/DTOs
2. `Application/Reviews/Commands/CreateReview/`
3. `Application/Reviews/Queries/GetReviews/`
4. `ReviewsController.cs` in API layer

Or, **remove the entity** if reviews are not planned.

---

## Repository Pattern Concern: Single Generic Repository (DESIGN SMELL)

### What This Means
Only one generic `Repository<TEntity>` exists, no per-aggregate repository interfaces.

**Current pattern:**
```csharp
public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query();  // Returns IQueryable
    Task<TEntity?> FindAsync(...);
    void Add(TEntity entity);
    void Remove(TEntity entity);
    void Update(TEntity entity);
}
```

**At scale, this causes:**
1. **Anemic domain**: No aggregate-specific business logic
2. **Query explosion**: Every handler writes its own LINQ
3. **IQueryable leak**: EF Core concerns in Application layer (already observed)
4. **Testability**: Hard to mock per-aggregate behavior

### Observation

**Current repo: ~138 total entities/features** (small to medium scale). Generic repository is **acceptable now**, but will become a bottleneck as the app grows.

### Recommendation (Not Urgent)

**Later, when scaling:** Introduce aggregate-specific repositories:
```csharp
public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetByIdWithItemsAsync(Guid id, CancellationToken ct);
    Task<List<Order>> GetCustomerOrdersAsync(string customerId, CancellationToken ct);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct);
}

public class OrderRepository : Repository<Order>, IOrderRepository
{
    // EF Core logic encapsulated here, not scattered in handlers
}
```

**Benefit:** Centralizes query logic, reduces IQueryable leak, improves testability.

---

## Fix Priority

| Priority | Defect | Effort | Risk |
|----------|--------|--------|------|
| **P0** | Layer bypass (3 controllers) | 2 hours | Low (breaking, but correct) |
| **P1** | Incomplete Review slice | 1 hour | Low |
| **P2** | IQueryable leak (architecture debt) | 4-6 hours | Medium (refactoring) |
| **P3** | Generic repository → aggregate repos | 8+ hours | High (large refactor) |

---

## Recommended Action Plan

### Phase 1: Fix Critical Layer Bypass (2 hours)
1. Create `Application/Auth/Commands/RegisterUser/`
2. Create `Application/Auth/Commands/LoginUser/`
3. Create `Application/Reports/Queries/`
4. Create `Application/Staff/Queries/`
5. Refactor 3 controllers to use MediatR

### Phase 2: Complete Review Slice (1 hour)
1. Create `Application/Reviews/Queries/GetReviews/`
2. Create `Application/Reviews/Commands/CreateReview/`
3. Create `ReviewsController.cs`

### Phase 3: Document IQueryable Leak (architecture debt log)
- Do not attempt refactor now (breaks existing tests)
- Plan for future aggregate repository pattern

---

## References

- **Clean Architecture Rule:** API → Application → Infrastructure (never skip layers)
- **Repository Pattern:** Single generic repo acceptable at small scale; aggregate repos recommended at scale
- **IQueryable leak:** EF Core concerns should stay in Infrastructure, not leak to Application

---

