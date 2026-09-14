using MediatR;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Features.Chefs.Commands.DeleteChef;

/// <summary>
/// Handler for DeleteChefCommand with cache invalidation.
/// ✅ Invalidates cache on delete (affects both individual chef and list caches)
/// </summary>
public class DeleteChefCommandHandler : IRequestHandler<DeleteChefCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public DeleteChefCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteChefCommand request, CancellationToken cancellationToken)
    {
        var chef = await _unitOfWork.Chefs.FindAsync(c => c.Id == request.Id, cancellationToken);
        if (chef is null)
            return false;

        _unitOfWork.Chefs.Remove(chef);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache: deleted chef affects both individual chef cache and list caches
        var chefCacheKey = string.Format(CacheKeys.ChefById, request.Id);
        await _cache.RemoveAsync(chefCacheKey, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Chefs, cancellationToken);

        return true;
    }
}
