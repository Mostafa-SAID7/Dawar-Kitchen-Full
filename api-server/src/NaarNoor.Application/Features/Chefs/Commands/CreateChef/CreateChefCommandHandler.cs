using MediatR;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Features.Chefs.Commands.CreateChef;

/// <summary>
/// Handler for CreateChefCommand with cache invalidation.
/// ✅ Invalidates chefs cache on create (affects GetChefs query)
/// </summary>
public class CreateChefCommandHandler : IRequestHandler<CreateChefCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public CreateChefCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateChefCommand request, CancellationToken cancellationToken)
    {
        var chef = new Chef
        {
            Name = request.Name,
            Title = request.Title,
            Bio = request.Bio,
            ImageUrl = request.ImageUrl,
            Specialty = request.Specialty,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder
        };

        _unitOfWork.Chefs.Add(chef);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache: new chef affects GetChefs list
        await _cache.RemoveAsync(CacheKeys.Chefs, cancellationToken);

        return chef.Id;
    }
}
