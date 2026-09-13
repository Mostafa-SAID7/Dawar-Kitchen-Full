using MediatR;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Features.Reservations.Queries.GetReservations;

public record GetReservationsQuery(int Page = 1, int PageSize = 20) : IRequest<List<ReservationDto>>;

