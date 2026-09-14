using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Features.Reservations.Commands.CreateReservation;

/// <summary>
/// Handler for CreateReservationCommand.
/// ✅ FIXED: Now centralizes date/time parsing logic (moved from controller).
/// Normalizes BookingTime OR ReservationDate/ReservationTime into clean domain values.
/// </summary>
public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // Normalize date and time from either BookingTime or separate fields
        DateOnly normalizedDate;
        TimeOnly normalizedTime;

        if (request.BookingTime.HasValue)
        {
            // BookingTime takes precedence: decompose into date + time
            normalizedDate = DateOnly.FromDateTime(request.BookingTime.Value);
            normalizedTime = TimeOnly.FromDateTime(request.BookingTime.Value);
        }
        else
        {
            // Use separate ReservationDate and ReservationTime
            normalizedDate = request.ReservationDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            
            if (!string.IsNullOrWhiteSpace(request.ReservationTime) &&
                TimeOnly.TryParse(request.ReservationTime, out var parsedTime))
            {
                normalizedTime = parsedTime;
            }
            else
            {
                normalizedTime = new TimeOnly(12, 0); // Default to noon if parsing fails
            }
        }

        // ✅ Use Reservation aggregate factory method (domain-driven)
        var reservation = Reservation.Create(
            customerName: request.CustomerName,
            email: request.Email,
            phoneNumber: request.PhoneNumber,
            reservationDate: normalizedDate,
            reservationTime: normalizedTime,
            partySize: request.PartySize,
            specialRequests: request.SpecialRequests
        );

        _unitOfWork.Reservations.Add(reservation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}

