using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;
using NaarNoor.Infrastructure.Data;

namespace NaarNoor.Infrastructure.Repositories;

/// <summary>
/// Review-specific repository implementation.
/// Extends generic Repository with Review-specific query methods.
/// Keeps Infrastructure layer (EF Core) separate from Application layer.
/// </summary>
public class ReviewRepository : Repository<Review>
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all approved reviews, ordered by most recent first.
    /// Used by GetApprovedReviewsQuery handler.
    /// </summary>
    public async Task<List<Review>> GetAllApprovedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all unapproved reviews pending moderation.
    /// Used by review moderation queries.
    /// </summary>
    public async Task<List<Review>> GetAllUnapprovedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => !r.IsApproved)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a paginated set of approved reviews.
    /// </summary>
    public async Task<(List<Review> Reviews, int TotalCount)> GetApprovedReviewsPaginatedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Reviews
            .Where(r => r.IsApproved)
            .CountAsync(cancellationToken);

        var reviews = await _context.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }
}
