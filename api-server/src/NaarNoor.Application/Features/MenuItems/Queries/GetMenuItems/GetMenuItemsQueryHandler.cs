using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs.MenuItems;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.Features.MenuItems.Queries.GetMenuItems;

/// <summary>
/// Query handler for retrieving menu items without caching.
/// Returns all available items, optionally filtered by category.
/// ✅ Fixed: Removed Microsoft.EntityFrameworkCore import
/// </summary>
public class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, List<MenuItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMenuItemsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MenuItemDto>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var allMenuItems = await _unitOfWork.MenuItems.GetAllAsync(cancellationToken);
        
        return allMenuItems
            .Where(m => m.IsAvailable)
            .ApplyCategoryFilter(request.Category)
            .ProjectToDto()
            .ToList();
    }
}

/// <summary>
/// Centralized projection and filtering for MenuItem entity → MenuItemDto.
/// Operates on LINQ-to-Objects (in-memory), not EF Core queries.
/// </summary>
internal static class MenuItemProjection
{
    internal static List<MenuItemDto> ProjectToDto(this IEnumerable<MenuItem> source)
        => source.Select(m => new MenuItemDto
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
        }).ToList();

    internal static IEnumerable<MenuItem> ApplyCategoryFilter(
        this IEnumerable<MenuItem> source, string? category)
    {
        if (string.IsNullOrWhiteSpace(category)) return source;
        return Enum.TryParse<MenuCategory>(category, ignoreCase: true, out var cat)
            ? source.Where(m => m.Category == cat)
            : source;
    }
}
