using MediatR;

namespace NaarNoor.Application.MenuItems.Commands.UpdateMenuItem;

public record UpdateMenuItemCommand(
    Guid Id,
    string? Name,
    string? Description,
    decimal? Price,
    string? Category,
    bool? IsVegetarian,
    bool? IsVegan,
    bool? IsGlutenFree,
    bool? IsAvailable,
    string? ImageUrl
) : IRequest<bool>;
