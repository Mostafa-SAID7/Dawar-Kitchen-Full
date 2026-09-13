using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMenuItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<MenuCategory>(request.Category, true, out var category))
            category = MenuCategory.Mains;

        var item = new MenuItem
        {
            Name        = request.Name,
            Description = request.Description ?? string.Empty,
            Price       = request.Price,
            Category    = category,
            IsVegetarian = request.IsVegetarian,
            IsVegan     = request.IsVegan,
            IsGlutenFree = request.IsGlutenFree,
            IsAvailable = request.IsAvailable,
            ImageUrl    = request.ImageUrl,
            SortOrder   = request.SortOrder,
        };

        _unitOfWork.MenuItems.Add(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
