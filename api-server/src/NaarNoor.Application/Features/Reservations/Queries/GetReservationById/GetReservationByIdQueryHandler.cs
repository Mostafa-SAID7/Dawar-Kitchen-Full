using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs.Reservations;

namespace NaarNoor.Application.Features.Reservations.Queries.GetReservationById;

/// <summary>
/// Handler for GetReservationByIdQuery
/// ✅ Fixed: Removed Microsoft.EntityFrameworkCore import
/// </summary>
public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.Id, cancellationToken);
        
        if (reservation is null)
            return null;

        return new ReservationDto
        {
            Id = reservation.Id,
            CustomerName = reservation.CustomerName,
            Email = reservation.Email,
            PhoneNumber = reservation.PhoneNumber,
            ReservationDate = reservation.ReservationDate,
            ReservationTime = reservation.ReservationTime.ToString("HH:mm"),
            PartySize = reservation.PartySize,
            Status = reservation.Status.ToString(),
            SpecialRequests = reservation.SpecialRequests,
            CreatedAt = reservation.CreatedAt
        };
    }
}
