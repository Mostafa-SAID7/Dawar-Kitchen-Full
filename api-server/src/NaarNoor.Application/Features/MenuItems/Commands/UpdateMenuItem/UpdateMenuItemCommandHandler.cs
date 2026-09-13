using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.MenuItems.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMenuItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.MenuItems.Query()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (item is null) return false;

        if (request.Name        is not null) item.Name        = request.Name;
        if (request.Description is not null) item.Description = request.Description;
        if (request.Price       is not null) item.Price       = request.Price.Value;
        if (request.IsAvailable is not null) item.IsAvailable = request.IsAvailable.Value;
        if (request.IsVegetarian is not null) item.IsVegetarian = request.IsVegetarian.Value;
        if (request.IsVegan     is not null) item.IsVegan     = request.IsVegan.Value;
        if (request.IsGlutenFree is not null) item.IsGlutenFree = request.IsGlutenFree.Value;
        if (request.ImageUrl    is not null) item.ImageUrl    = request.ImageUrl;
        if (request.Category    is not null &&
            Enum.TryParse<MenuCategory>(request.Category, true, out var cat))
            item.Category = cat;

        item.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.MenuItems.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
