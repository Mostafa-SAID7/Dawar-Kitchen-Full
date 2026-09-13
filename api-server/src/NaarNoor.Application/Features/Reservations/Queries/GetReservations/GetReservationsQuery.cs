using MediatR;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Reservations.Queries.GetReservations;

public record GetReservationsQuery(int Page = 1, int PageSize = 20) : IRequest<List<ReservationDto>>;
