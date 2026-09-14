using MediatR;

namespace NaarNoor.Application.Features.Chefs.Commands.CreateChef;

/// <summary>
/// Command to create a new chef
/// </summary>
public record CreateChefCommand(
    string Name,
    string Title,
    string Bio,
    string? ImageUrl,
    string? Specialty,
    bool IsActive,
    int SortOrder
) : IRequest<Guid>;
