using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Reservations.Queries.GetReservationById;

public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Reservations.Query()
            .Where(r => r.Id == request.Id)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                CustomerName = r.CustomerName,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                ReservationDate = r.ReservationDate,
                ReservationTime = r.ReservationTime.ToString("HH:mm"),
                PartySize = r.PartySize,
                Status = r.Status.ToString(),
                SpecialRequests = r.SpecialRequests,
                CreatedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
