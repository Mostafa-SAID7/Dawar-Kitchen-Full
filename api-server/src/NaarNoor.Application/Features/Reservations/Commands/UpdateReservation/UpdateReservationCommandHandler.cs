using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.Reservations.Commands.UpdateReservation;

public class UpdateReservationCommandHandler : IRequestHandler<UpdateReservationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.Query()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (reservation is null) return false;

        if (request.CustomerName  is not null) reservation.CustomerName  = request.CustomerName;
        if (request.Email         is not null) reservation.Email         = request.Email;
        if (request.PhoneNumber   is not null) reservation.PhoneNumber   = request.PhoneNumber;
        if (request.PartySize     is not null) reservation.PartySize     = request.PartySize.Value;
        if (request.SpecialRequests is not null) reservation.SpecialRequests = request.SpecialRequests;

        if (request.Status is not null &&
            Enum.TryParse<ReservationStatus>(request.Status, true, out var newStatus))
        {
            try { reservation.TransitionTo(newStatus); }
            catch (InvalidOperationException) { /* ignore invalid transitions */ }
        }

        reservation.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Reservations.Update(reservation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
