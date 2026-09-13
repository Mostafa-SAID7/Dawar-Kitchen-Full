using MediatR;

namespace NaarNoor.Application.MenuItems.Commands.CreateMenuItem;

public record CreateMenuItemCommand(
    string Name,
    string? Description,
    decimal Price,
    string Category,
    bool IsVegetarian,
    bool IsVegan,
    bool IsGlutenFree,
    bool IsAvailable,
    string? ImageUrl,
    int SortOrder
) : IRequest<Guid>;
