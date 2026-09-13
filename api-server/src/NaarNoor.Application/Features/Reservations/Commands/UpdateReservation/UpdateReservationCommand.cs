using MediatR;

namespace NaarNoor.Application.Reservations.Commands.UpdateReservation;

public record UpdateReservationCommand(
    Guid Id,
    string? CustomerName,
    string? Email,
    string? PhoneNumber,
    string? Status,
    int? PartySize,
    string? SpecialRequests
) : IRequest<bool>;
