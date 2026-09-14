using MediatR;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Features.MenuItems.Commands.DeleteMenuItem;

/// <summary>
/// Handler for DeleteMenuItemCommand with cache invalidation.
/// ✅ Invalidates cache on delete (affects both individual item and list caches)
/// </summary>
public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public DeleteMenuItemCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.MenuItems.FindAsync(m => m.Id == request.Id, cancellationToken);
        if (item is null)
            return false;

        _unitOfWork.MenuItems.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache: deleted item affects both individual item cache and list caches
        var itemCacheKey = string.Format(CacheKeys.MenuItemById, request.Id);
        await _cache.RemoveAsync(itemCacheKey, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.MenuItems, cancellationToken);

        return true;
    }
}
