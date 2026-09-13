using System.Linq.Expressions;

namespace NaarNoor.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface for data access.
/// Provides basic CRUD operations and entity lookups.
/// 
/// NOTE: IQueryable is only available via Query() in Infrastructure implementations.
/// Application handlers should use explicit domain-specific repository methods
/// (e.g., IMenuItemRepository.GetAvailableByIdsAsync) to maintain clean architecture.
/// </summary>
public interface IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Returns a queryable source. 
    /// WARNING: This is meant for Infrastructure internal use and aggregate-specific repository implementations.
    /// Application handlers should prefer explicit methods (GetByIdAsync, GetAvailableAsync, etc.)
    /// to maintain separation from EF Core details.
    /// </summary>
    IQueryable<TEntity> Query();

    /// <summary>
    /// Returns the first entity matching the predicate, or null.
    /// Use for single-entity lookups in commands (update / delete).
    /// </summary>
    Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first entity with the given ID, or null.
    /// Convenience method for GetById queries.
    /// </summary>
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities (all rows in table).
    /// Use sparingly for small reference data; prefer filtered queries for large tables.
    /// </summary>
    Task<List<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    void Add(TEntity entity);
    void Remove(TEntity entity);
    void Update(TEntity entity);
}
