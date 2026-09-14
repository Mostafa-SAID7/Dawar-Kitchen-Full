using MediatR;
using NaarNoor.Application.DTOs.Chefs;

namespace NaarNoor.Application.Features.Chefs.Queries.GetChefs;

public record GetChefsQuery : IRequest<List<ChefDto>>;

