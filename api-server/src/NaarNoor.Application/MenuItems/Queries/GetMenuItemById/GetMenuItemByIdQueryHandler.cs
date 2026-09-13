using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItemById;

public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, MenuItemDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMenuItemByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MenuItemDto?> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.MenuItems.Query()
            .Where(m => m.Id == request.Id)
            .Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                Category = m.Category.ToString(),
                IsVegetarian = m.IsVegetarian,
                IsVegan = m.IsVegan,
                IsGlutenFree = m.IsGlutenFree,
                IsAvailable = m.IsAvailable,
                ImageUrl = m.ImageUrl,
                SortOrder = m.SortOrder
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
