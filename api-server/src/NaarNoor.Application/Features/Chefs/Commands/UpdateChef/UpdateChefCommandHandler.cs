using MediatR;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Features.Chefs.Commands.UpdateChef;

/// <summary>
/// Handler for UpdateChefCommand with cache invalidation.
/// ✅ Invalidates cache on update (affects both individual chef and list caches)
/// </summary>
public class UpdateChefCommandHandler : IRequestHandler<UpdateChefCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public UpdateChefCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateChefCommand request, CancellationToken cancellationToken)
    {
        var chef = await _unitOfWork.Chefs.FindAsync(c => c.Id == request.Id, cancellationToken);
        if (chef is null)
            return false;

        // Update only provided fields (partial update pattern)
        if (request.Name is not null)
            chef.Name = request.Name;
        
        if (request.Title is not null)
            chef.Title = request.Title;
        
        if (request.Bio is not null)
            chef.Bio = request.Bio;
        
        if (request.ImageUrl is not null)
            chef.ImageUrl = request.ImageUrl;
        
        if (request.Specialty is not null)
            chef.Specialty = request.Specialty;
        
        if (request.IsActive.HasValue)
            chef.IsActive = request.IsActive.Value;

        chef.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Chefs.Update(chef);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache: updated chef affects both individual chef cache and list caches
        var chefCacheKey = string.Format(CacheKeys.ChefById, request.Id);
        await _cache.RemoveAsync(chefCacheKey, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Chefs, cancellationToken);

        return true;
    }
}
