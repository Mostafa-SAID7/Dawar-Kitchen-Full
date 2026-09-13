using MediatR;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Features.Reservations.Queries.GetReservationById;

public record GetReservationByIdQuery(Guid Id) : IRequest<ReservationDto?>;

