using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItems;

/// <summary>
/// Query handler for retrieving menu items without caching.
/// Returns all available items, optionally filtered by category.
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
        var menuItems = _unitOfWork.MenuItems.Query()
            .Where(m => m.IsAvailable)
            .FilterByCategory(request.Category)
            .ProjectToDto();

        return await menuItems.ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Centralized EF projection from MenuItem entity → MenuItemDto.
/// Used by all menu item query handlers to avoid duplicating Select(...) expressions.
/// </summary>
internal static class MenuItemProjection
{
    internal static IQueryable<MenuItemDto> ProjectToDto(this IQueryable<MenuItem> source)
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
        });

    internal static IQueryable<MenuItem> FilterByCategory(
        this IQueryable<MenuItem> source, string? category)
    {
        if (string.IsNullOrWhiteSpace(category)) return source;
        return Enum.TryParse<MenuCategory>(category, ignoreCase: true, out var cat)
            ? source.Where(m => m.Category == cat)
            : source;
    }
}
