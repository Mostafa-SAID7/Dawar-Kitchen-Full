using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Infrastructure.Data;

namespace NaarNoor.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation.
/// Encapsulates EF Core data access logic within Infrastructure layer.
/// Application handlers should use explicit domain-specific repository methods
/// rather than directly calling Query() to maintain clean architecture.
/// </summary>
public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns an IQueryable source for LINQ composition.
    /// Intended for Infrastructure-internal use and aggregate-specific repositories.
    /// Application handlers should avoid direct use; prefer explicit methods.
    /// </summary>
    public IQueryable<TEntity> Query()
        => _context.Set<TEntity>();

    public Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);

    public async Task<List<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().ToListAsync(cancellationToken);

    public void Add(TEntity entity)
        => _context.Set<TEntity>().Add(entity);

    public void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);
}
