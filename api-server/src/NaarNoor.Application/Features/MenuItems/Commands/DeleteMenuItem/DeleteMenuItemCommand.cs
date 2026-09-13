using MediatR;

namespace NaarNoor.Application.Features.MenuItems.Commands.DeleteMenuItem;

public record DeleteMenuItemCommand(Guid Id) : IRequest<bool>;

