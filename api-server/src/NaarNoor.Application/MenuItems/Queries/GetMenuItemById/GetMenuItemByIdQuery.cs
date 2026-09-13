using MediatR;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItemById;

public record GetMenuItemByIdQuery(Guid Id) : IRequest<MenuItemDto?>;
