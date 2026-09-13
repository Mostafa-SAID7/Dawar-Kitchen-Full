using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Features.Reservations.Commands.CreateReservation;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // ✅ Use Reservation aggregate factory method (domain-driven)
        var reservation = Reservation.Create(
            customerName: request.CustomerName,
            email: request.Email,
            phoneNumber: request.PhoneNumber,
            reservationDate: request.ReservationDate,
            reservationTime: TimeOnly.Parse(request.ReservationTime),
            partySize: request.PartySize,
            specialRequests: request.SpecialRequests
        );

        _unitOfWork.Reservations.Add(reservation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}

