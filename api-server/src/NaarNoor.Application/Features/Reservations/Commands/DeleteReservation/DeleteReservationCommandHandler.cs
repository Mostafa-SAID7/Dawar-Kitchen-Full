using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Reservations.Commands.DeleteReservation;

public class DeleteReservationCommandHandler : IRequestHandler<DeleteReservationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.Query()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (reservation is null) return false;

        _unitOfWork.Reservations.Remove(reservation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
