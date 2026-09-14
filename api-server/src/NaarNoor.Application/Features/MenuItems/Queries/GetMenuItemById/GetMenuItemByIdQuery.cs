using MediatR;
using NaarNoor.Application.DTOs.MenuItems;

namespace NaarNoor.Application.Features.MenuItems.Queries.GetMenuItemById;

public record GetMenuItemByIdQuery(Guid Id) : IRequest<MenuItemDto?>;

