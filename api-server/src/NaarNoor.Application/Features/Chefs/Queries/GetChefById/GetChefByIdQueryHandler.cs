using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs.Chefs;

namespace NaarNoor.Application.Features.Chefs.Queries.GetChefById;

/// <summary>
/// Handler for GetChefByIdQuery.
/// ✅ Implements MediatR query pattern: Routes single-chef fetch through clean architecture layer
/// ✅ Filtering & mapping centralized in handler, not in controller
/// </summary>
public class GetChefByIdQueryHandler : IRequestHandler<GetChefByIdQuery, ChefDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetChefByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ChefDto?> Handle(GetChefByIdQuery request, CancellationToken cancellationToken)
    {
        var chef = await _unitOfWork.Chefs.GetByIdAsync(request.Id, cancellationToken);

        // Return null if chef not found or inactive
        if (chef is null || !chef.IsActive)
            return null;

        return new ChefDto
        {
            Id = chef.Id,
            Name = chef.Name,
            Title = chef.Title,
            Bio = chef.Bio,
            ImageUrl = chef.ImageUrl,
            Specialty = chef.Specialty,
            SortOrder = chef.SortOrder
        };
    }
}
