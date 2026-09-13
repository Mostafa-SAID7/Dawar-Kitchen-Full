using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Chefs.Queries.GetChefs;

public class GetChefsQueryHandler : IRequestHandler<GetChefsQuery, List<ChefDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetChefsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ChefDto>> Handle(GetChefsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Chefs.Query()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new ChefDto
            {
                Id = c.Id,
                Name = c.Name,
                Title = c.Title,
                Bio = c.Bio,
                ImageUrl = c.ImageUrl,
                Specialty = c.Specialty,
                SortOrder = c.SortOrder
            })
            .ToListAsync(cancellationToken);
    }
}
