using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs.Reservations;

namespace NaarNoor.Application.Features.Reservations.Queries.GetReservations;

/// <summary>
/// Handler for GetReservationsQuery
/// ✅ Fixed: Removed Microsoft.EntityFrameworkCore import
/// Note: Pagination done in-memory (acceptable for reference tables; optimize later if needed)
/// </summary>
public class GetReservationsQueryHandler : IRequestHandler<GetReservationsQuery, List<ReservationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ReservationDto>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
    {
        var allReservations = await _unitOfWork.Reservations.GetAllAsync(cancellationToken);
        
        return allReservations
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
            .ToList();
    }
}
