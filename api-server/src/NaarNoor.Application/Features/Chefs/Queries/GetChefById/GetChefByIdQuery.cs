using MediatR;
using NaarNoor.Application.DTOs.Chefs;

namespace NaarNoor.Application.Features.Chefs.Queries.GetChefById;

/// <summary>
/// Query to fetch a single chef by ID.
/// Returns null if chef not found or inactive.
/// </summary>
public record GetChefByIdQuery(Guid Id) : IRequest<ChefDto?>;
