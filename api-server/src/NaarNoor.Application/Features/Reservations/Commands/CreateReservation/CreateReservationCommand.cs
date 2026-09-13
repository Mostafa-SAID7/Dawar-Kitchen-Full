using MediatR;

namespace NaarNoor.Application.Reservations.Commands.CreateReservation;

public record CreateReservationCommand(
    string CustomerName,
    string Email,
    string PhoneNumber,
    DateOnly ReservationDate,
    string ReservationTime,
    int PartySize,
    string? SpecialRequests
) : IRequest<Guid>;
