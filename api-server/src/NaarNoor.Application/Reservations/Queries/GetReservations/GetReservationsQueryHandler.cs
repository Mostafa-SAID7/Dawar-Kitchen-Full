using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;

namespace NaarNoor.Application.Reservations.Queries.GetReservations;

public class GetReservationsQueryHandler : IRequestHandler<GetReservationsQuery, List<ReservationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReservationDto>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Reservations.Query()
            .OrderByDescending(r => r.ReservationDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);
    }
}
