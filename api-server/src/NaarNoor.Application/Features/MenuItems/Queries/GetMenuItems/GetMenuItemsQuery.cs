using MediatR;
using NaarNoor.Application.DTOs.MenuItems;

namespace NaarNoor.Application.Features.MenuItems.Queries.GetMenuItems;

public record GetMenuItemsQuery(string? Category = null) : IRequest<List<MenuItemDto>>;

