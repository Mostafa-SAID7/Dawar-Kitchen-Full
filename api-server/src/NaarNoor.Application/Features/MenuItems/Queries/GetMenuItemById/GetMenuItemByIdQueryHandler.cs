using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs.MenuItems;

namespace NaarNoor.Application.Features.MenuItems.Queries.GetMenuItemById;

/// <summary>
/// Handler for GetMenuItemByIdQuery
/// ✅ Fixed: Removed Microsoft.EntityFrameworkCore import
/// </summary>
public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, MenuItemDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMenuItemByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MenuItemDto?> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
    {
        var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(request.Id, cancellationToken);
        
        if (menuItem is null)
            return null;

        return new MenuItemDto
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Description = menuItem.Description,
            Price = menuItem.Price,
            Category = menuItem.Category.ToString(),
            IsVegetarian = menuItem.IsVegetarian,
            IsVegan = menuItem.IsVegan,
            IsGlutenFree = menuItem.IsGlutenFree,
            IsAvailable = menuItem.IsAvailable,
            ImageUrl = menuItem.ImageUrl,
            SortOrder = menuItem.SortOrder
        };
    }
}
