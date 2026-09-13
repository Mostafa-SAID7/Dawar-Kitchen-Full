using MediatR;

namespace NaarNoor.Application.Features.Reservations.Commands.CreateReservation;

public record CreateReservationCommand(
    string CustomerName,
    string Email,
    string PhoneNumber,
    DateOnly ReservationDate,
    string ReservationTime,
    int PartySize,
    string? SpecialRequests
) : IRequest<Guid>;

