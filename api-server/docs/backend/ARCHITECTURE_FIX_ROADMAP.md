# Architecture Defect Fix Roadmap
## Clean Architecture Restoration Plan

**Total Estimated Effort:** 7-9 hours across 3 phases  
**Priority:** P0 (critical) → P1 (high) → P2 (debt log)

---

## Phase 1: Fix Critical Layer Bypass (IMMEDIATE)

**Defect:** AuthController, ReportsController, StaffController call Infrastructure directly  
**Impact:** Clean Architecture violation, hard to test, business logic in HTTP layer  
**Effort:** ~2 hours  
**Deliverable:** All controllers use MediatR

### 1.1 Create Auth Application Commands

**File:** `api-server/src/NaarNoor.Application/Auth/Commands/RegisterUser/RegisterUserCommand.cs`

```csharp
using MediatR;

namespace NaarNoor.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FullName
) : IRequest<RegisterUserResult>;

public record RegisterUserResult(
    string UserId,
    string Email,
    bool Success
);
```

**File:** `api-server/src/NaarNoor.Application/Auth/Commands/RegisterUser/RegisterUserCommandValidator.cs`

```csharp
using FluentValidation;

namespace NaarNoor.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Full name is required and max 100 chars.");
    }
}
```

**File:** `api-server/src/NaarNoor.Application/Auth/Commands/RegisterUser/RegisterUserCommandHandler.cs`

```csharp
using MediatR;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserService _userService;

    public RegisterUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName,
            cancellationToken);

        if (!result.Success)
            throw new InvalidOperationException(result.Error ?? "Registration failed.");

        return new RegisterUserResult(
            result.User!.Id,
            result.User.Email,
            true);
    }
}
```

### 1.2 Create Auth Application Query

**File:** `api-server/src/NaarNoor.Application/Auth/Queries/LoginUser/LoginUserQuery.cs`

```csharp
using MediatR;

namespace NaarNoor.Application.Auth.Queries.LoginUser;

public record LoginUserQuery(
    string Email,
    string Password
) : IRequest<LoginUserResult>;

public record LoginUserResult(
    string UserId,
    string Email,
    string AccessToken,
    string? RefreshToken = null
);
```

**File:** `api-server/src/NaarNoor.Application/Auth/Queries/LoginUser/LoginUserQueryHandler.cs`

```csharp
using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Infrastructure.Services;

namespace NaarNoor.Application.Auth.Queries.LoginUser;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginUserResult>
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public LoginUserQueryHandler(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);

        return new LoginUserResult(
            user.Id,
            user.Email,
            token);
    }
}
```

### 1.3 Refactor AuthController

**File:** `api-server/src/NaarNoor.API/Controllers/AuthController.cs` (REPLACE)

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Auth.Commands.RegisterUser;
using NaarNoor.Application.Auth.Queries.LoginUser;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] AuthRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FullName ?? "");

        var result = await _mediator.Send(command, cancellationToken);
        return Created("", new { userId = result.UserId });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] AuthLoginRequest request,
        CancellationToken cancellationToken)
    {
        var query = new LoginUserQuery(request.Email, request.Password);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new
        {
            access_token = result.AccessToken,
            user_id = result.UserId,
            email = result.Email
        });
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetMe()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;

        return Ok(new { userId, email });
    }
}

public class AuthRegisterRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? FullName { get; set; }
}

public class AuthLoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
```

### 1.4 Create Reports Application Queries

**File:** `api-server/src/NaarNoor.Application/Reports/Queries/GetRevenueReport/GetRevenueReportQuery.cs`

```csharp
using MediatR;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Reports.Queries.GetRevenueReport;

public record GetRevenueReportQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<RevenueReportDto>;
```

**File:** `api-server/src/NaarNoor.Application/Reports/Queries/GetRevenueReport/GetRevenueReportQueryHandler.cs`

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Reports.Queries.GetRevenueReport;

public class GetRevenueReportQueryHandler : IRequestHandler<GetRevenueReportQuery, RevenueReportDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRevenueReportQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RevenueReportDto> Handle(
        GetRevenueReportQuery request,
        CancellationToken cancellationToken)
    {
        var from = request.FromDate ?? DateTime.UtcNow.AddYears(-1);
        var to = request.ToDate ?? DateTime.UtcNow;

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var yearStart = new DateTime(today.Year, 1, 1);

        var allOrders = await _unitOfWork.Orders.Query()
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .Select(o => new { o.TotalAmount, o.CreatedAt })
            .ToListAsync(cancellationToken);

        return new RevenueReportDto
        {
            TodayRevenue = allOrders.Where(o => o.CreatedAt.Date == today).Sum(o => o.TotalAmount),
            WeekRevenue = allOrders.Where(o => o.CreatedAt >= weekStart).Sum(o => o.TotalAmount),
            MonthRevenue = allOrders.Where(o => o.CreatedAt >= monthStart).Sum(o => o.TotalAmount),
            YearRevenue = allOrders.Where(o => o.CreatedAt >= yearStart).Sum(o => o.TotalAmount),
            AveragePerOrder = allOrders.Count > 0 ? allOrders.Average(o => (double)o.TotalAmount) : 0,
        };
    }
}
```

(Continue pattern for `GetOrderStats`, `GetReservationStats`, `GetMenuReport`)

### 1.5 Create Staff Application Queries

**File:** `api-server/src/NaarNoor.Application/Staff/Queries/GetAllStaff/GetAllStaffQuery.cs`

```csharp
using MediatR;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Staff.Queries.GetAllStaff;

public record GetAllStaffQuery : IRequest<List<StaffDto>>;
```

(Continue pattern with handler...)

### 1.6 Refactor ReportsController & StaffController

Replace with thin HTTP mapping using MediatR (pattern: POST /api/orders).

---

## Phase 2: Complete Review Entity Slice (1 hour)

**Defect:** Review.cs entity exists but has no Application layer  
**Impact:** Incomplete feature, database table with no API  
**Effort:** ~1 hour

### 2.1 Create ReviewDto

**File:** `api-server/src/NaarNoor.Application/DTOs/ReviewDto.cs`

```csharp
namespace NaarNoor.Application.DTOs;

public class ReviewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public Guid MenuItemId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 2.2 Create Review Queries

**Files:**
- `Application/Reviews/Queries/GetReviews/GetReviewsQuery.cs`
- `Application/Reviews/Queries/GetReviews/GetReviewsQueryHandler.cs`
- `Application/Reviews/Queries/GetReviewById/GetReviewByIdQuery.cs`
- `Application/Reviews/Queries/GetReviewById/GetReviewByIdQueryHandler.cs`

### 2.3 Create Review Commands

**Files:**
- `Application/Reviews/Commands/CreateReview/CreateReviewCommand.cs`
- `Application/Reviews/Commands/CreateReview/CreateReviewCommandValidator.cs`
- `Application/Reviews/Commands/CreateReview/CreateReviewCommandHandler.cs`

### 2.4 Create ReviewsController

**File:** `api-server/src/NaarNoor.API/Controllers/ReviewsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet("menu/{menuItemId:guid}")]
    public async Task<IActionResult> GetMenuItemReviews(Guid menuItemId, CancellationToken ct)
    {
        var query = new GetReviewsQuery(menuItemId);
        var reviews = await _mediator.Send(query, ct);
        return Ok(reviews);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview(
        [FromBody] CreateReviewRequest request,
        CancellationToken ct)
    {
        var command = new CreateReviewCommand(
            request.MenuItemId,
            request.Title,
            request.Content,
            request.Rating,
            request.AuthorName);

        var review = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetMenuItemReviews), new { menuItemId = review.MenuItemId }, review);
    }
}
```

---

## Phase 3: Document IQueryable Leak (Architecture Debt Log)

**Defect:** EF Core IQueryable exposed in Repository.Query()  
**Impact:** Application layer tightly coupled to EF Core  
**Effort:** 0 hours now (document only); 4-6 hours for later refactor

### 3.1 Create Architecture Debt Log

**File:** `api-server/docs/backend/ARCHITECTURE_DEBT.md`

```markdown
# Architecture Debt Log

## Issue: IQueryable Leak in Repository Pattern

**Severity:** High (long-term debt)  
**Target Resolution:** Next major refactor  
**Estimated Effort:** 4-6 hours

### Current Pattern (Problematic)

\`\`\`csharp
public IQueryable<TEntity> Query() => _context.Set<TEntity>();
\`\`\`

### Issue

- Application handlers call `.ToListAsync()`, `.FirstOrDefaultAsync()` directly
- EF Core concerns leak into Application layer
- Difficult to test (must mock IQueryable behavior)
- Can't switch database providers

### Recommended Future Fix

Introduce aggregate-specific repositories:

\`\`\`csharp
public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetByIdWithItemsAsync(Guid id, CancellationToken ct);
    Task<List<Order>> GetCustomerOrdersAsync(string customerId, CancellationToken ct);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct);
}
\`\`\`

### Migration Path

1. Keep generic Repository for small entities (Chef, MenuItem)
2. Create aggregate repositories for Orders, Reservations, Users
3. Move EF Core logic from handlers to repositories
4. Return strongly-typed Task<T> instead of IQueryable

---
```

---

## Implementation Checklist

### Phase 1: Layer Bypass Fix
- [ ] Create Auth Commands (RegisterUser, LoginUser)
- [ ] Create Auth Command Validators
- [ ] Create Auth Command Handlers
- [ ] Create Reports Query handlers (4 queries)
- [ ] Create Staff Query handlers (2 queries)
- [ ] Refactor AuthController
- [ ] Refactor ReportsController
- [ ] Refactor StaffController
- [ ] Build & verify: 0 errors
- [ ] Run tests
- [ ] Commit: "fix(arch): eliminate layer bypass violations in Auth, Reports, Staff"

### Phase 2: Complete Review Slice
- [ ] Create ReviewDto
- [ ] Create Review Queries (2 files)
- [ ] Create Review Command (3 files)
- [ ] Create ReviewsController
- [ ] Update UnitOfWork with IRepository<Review>
- [ ] Update DbContext with DbSet<Review>
- [ ] Build & verify: 0 errors
- [ ] Commit: "feat(reviews): complete Review entity vertical slice with API"

### Phase 3: Document Debt
- [ ] Create ARCHITECTURE_DEBT.md
- [ ] Add note to DependencyInjection.cs
- [ ] Commit: "docs(arch): log IQueryable leak as future refactoring item"

---

## Testing Strategy

**Phase 1:**
- Unit tests for Auth commands/queries
- Unit tests for Report queries
- Integration tests: AuthController endpoints
- Integration tests: ReportsController endpoints

**Phase 2:**
- Unit tests for Review commands/queries
- Integration tests: ReviewsController endpoints

**Phase 3:**
- No testing needed (documentation only)

---

## Risk Assessment

| Phase | Risk | Mitigation |
|-------|------|-----------|
| 1 | Breaking change to Auth endpoints | Backward compatible (same HTTP contract) |
| 2 | Adding Review feature mid-dev | Isolated, no impact on existing features |
| 3 | Documenting debt (no code change) | Zero risk |

---

## Timeline

**Phase 1:** 2 hours (highest priority)  
**Phase 2:** 1 hour (complete in-flight feature)  
**Phase 3:** 0.5 hours (document technical debt)

**Total:** 3.5 hours to fix all critical issues

---

