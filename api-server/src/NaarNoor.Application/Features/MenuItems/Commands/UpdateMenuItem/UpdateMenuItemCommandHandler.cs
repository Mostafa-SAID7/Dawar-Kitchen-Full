using MediatR;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.Features.MenuItems.Commands.UpdateMenuItem;

/// <summary>
/// Handler for UpdateMenuItemCommand with cache invalidation.
/// ✅ Invalidates cache on update (affects both individual item and list caches)
/// </summary>
public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public UpdateMenuItemCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.MenuItems.FindAsync(m => m.Id == request.Id, cancellationToken);
        if (item is null)
            return false;

        // Update only provided fields (partial update pattern)
        if (request.Name is not null)
            item.Name = request.Name;
        
        if (request.Description is not null)
            item.Description = request.Description;
        
        if (request.Price.HasValue)
            item.Price = request.Price.Value;
        
        if (request.Category is not null)
        {
            if (Enum.TryParse<MenuCategory>(request.Category, true, out var category))
                item.Category = category;
        }
        
        if (request.IsVegetarian.HasValue)
            item.IsVegetarian = request.IsVegetarian.Value;
        
        if (request.IsVegan.HasValue)
            item.IsVegan = request.IsVegan.Value;
        
        if (request.IsGlutenFree.HasValue)
            item.IsGlutenFree = request.IsGlutenFree.Value;
        
        if (request.IsAvailable.HasValue)
            item.IsAvailable = request.IsAvailable.Value;
        
        if (request.ImageUrl is not null)
            item.ImageUrl = request.ImageUrl;

        item.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.MenuItems.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache: updated item affects both individual item cache and list caches
        var itemCacheKey = string.Format(CacheKeys.MenuItemById, request.Id);
        await _cache.RemoveAsync(itemCacheKey, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.MenuItems, cancellationToken);

        return true;
    }
}
