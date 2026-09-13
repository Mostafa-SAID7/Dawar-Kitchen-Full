using MediatR;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Features.MenuItems.Queries.GetMenuItems;

/// <summary>
/// Handles GetMenuItemsQuery with a 5-minute distributed cache layer.
/// Cache hit: ~5-10 ms | Cache miss: ~80-100 ms | Expected hit rate: 85-95%.
/// Category-filtered requests bypass the cache (too many key variations).
/// ✅ Fixed: Removed Microsoft.EntityFrameworkCore import
/// </summary>
public class GetMenuItemsCachedQueryHandler : IRequestHandler<GetMenuItemsQuery, List<MenuItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public GetMenuItemsCachedQueryHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<List<MenuItemDto>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Category))
            return await QueryDatabaseAsync(request, cancellationToken);

        var cached = await _cache.GetAsync<List<MenuItemDto>>(CacheKeys.MenuItems, cancellationToken);
        if (cached is not null) return cached;

        var items = await QueryDatabaseAsync(request, cancellationToken);
        await _cache.SetAsync(CacheKeys.MenuItems, items, CacheKeys.GetExpiration(CacheKeys.MenuItems), cancellationToken);
        return items;
    }

    private async Task<List<MenuItemDto>> QueryDatabaseAsync(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var allMenuItems = await _unitOfWork.MenuItems.GetAllAsync(cancellationToken);
        
        return allMenuItems
            .Where(m => m.IsAvailable)
            .ApplyCategoryFilter(request.Category)
            .OrderBy(m => m.Category)
            .ThenBy(m => m.SortOrder)
            .ProjectToDto()
            .ToList();
    }
}
