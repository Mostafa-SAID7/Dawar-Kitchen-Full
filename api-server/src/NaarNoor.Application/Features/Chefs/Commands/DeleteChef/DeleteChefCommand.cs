using MediatR;

namespace NaarNoor.Application.Features.Chefs.Commands.DeleteChef;

/// <summary>
/// Command to delete a chef
/// </summary>
public record DeleteChefCommand(Guid Id) : IRequest<bool>;
