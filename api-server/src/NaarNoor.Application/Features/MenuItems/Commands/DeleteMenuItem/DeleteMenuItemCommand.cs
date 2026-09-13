using MediatR;

namespace NaarNoor.Application.MenuItems.Commands.DeleteMenuItem;

public record DeleteMenuItemCommand(Guid Id) : IRequest<bool>;
