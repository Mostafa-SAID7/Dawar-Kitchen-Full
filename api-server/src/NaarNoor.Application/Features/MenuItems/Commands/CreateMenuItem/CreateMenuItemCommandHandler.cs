using MediatR;
using NaarNoor.Application.Caching;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.Features.MenuItems.Commands.CreateMenuItem;

/// <summary>
/// Handler for CreateMenuItemCommand with cache invalidation.
/// ✅ Invalidates menu items cache on create (affects GetMenuItems and category filters)
/// </summary>
public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public CreateMenuItemCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        // Parse category with fallback to Mains
        if (!Enum.TryParse<MenuCategory>(request.Category, true, out var category))
            category = MenuCategory.Mains;

        var menuItem = new MenuItem
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Price = request.Price,
            Category = category,
            IsVegetarian = request.IsVegetarian,
            IsVegan = request.IsVegan,
            IsGlutenFree = request.IsGlutenFree,
            IsAvailable = request.IsAvailable,
            ImageUrl = request.ImageUrl,
            SortOrder = request.SortOrder
        };

        _unitOfWork.MenuItems.Add(menuItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache: new item affects all menu items lists
        await _cache.RemoveAsync(CacheKeys.MenuItems, cancellationToken);

        return menuItem.Id;
    }
}
