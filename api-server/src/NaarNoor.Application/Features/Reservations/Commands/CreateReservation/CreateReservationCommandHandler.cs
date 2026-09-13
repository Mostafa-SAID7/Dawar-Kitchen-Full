using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Reservations.Commands.CreateReservation;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = new Reservation
        {
            CustomerName = request.CustomerName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ReservationDate = request.ReservationDate,
            ReservationTime = TimeOnly.Parse(request.ReservationTime),
            PartySize = request.PartySize,
            SpecialRequests = request.SpecialRequests
        };

        _unitOfWork.Reservations.Add(reservation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
