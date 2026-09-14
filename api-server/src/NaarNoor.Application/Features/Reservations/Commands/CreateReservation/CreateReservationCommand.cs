using MediatR;

namespace NaarNoor.Application.Features.Reservations.Commands.CreateReservation;

/// <summary>
/// Command to create a new reservation.
/// ✅ FIXED: Now accepts raw date/time fields from controller; handler normalizes them.
/// Supports both BookingTime (DateTime) and separate ReservationDate/ReservationTime (DateOnly/string).
/// </summary>
public record CreateReservationCommand(
    string CustomerName,
    string Email,
    string PhoneNumber,
    DateTime? BookingTime,
    DateOnly? ReservationDate,
    string? ReservationTime,
    int PartySize,
    string? SpecialRequests
) : IRequest<Guid>;

