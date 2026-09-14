using MediatR;

namespace NaarNoor.Application.Features.Chefs.Commands.UpdateChef;

/// <summary>
/// Command to update an existing chef (partial update)
/// </summary>
public record UpdateChefCommand(
    Guid Id,
    string? Name,
    string? Title,
    string? Bio,
    string? ImageUrl,
    string? Specialty,
    bool? IsActive
) : IRequest<bool>;
