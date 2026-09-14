using MediatR;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Features.Reviews.Queries.GetApprovedReviews;

/// <summary>
/// Handler for GetApprovedReviewsQuery.
/// Retrieves paginated approved reviews for public display.
/// ✅ Uses IUnitOfWork for repository access
/// ✅ No EF Core in Application layer — repository handles filtering
/// </summary>
public class GetApprovedReviewsHandler : IRequestHandler<GetApprovedReviewsQuery, GetApprovedReviewsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetApprovedReviewsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetApprovedReviewsResponse> Handle(
        GetApprovedReviewsQuery request,
        CancellationToken cancellationToken)
    {
        // Get all approved reviews using the repository's Query() method
        // The Query() returns IQueryable which we then filter
        var query = _unitOfWork.Reviews.Query()
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt);

        // Get total count
        var totalCount = query.Count();

        // Apply pagination
        var paginatedReviews = query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();

        // Map to DTOs
        var reviewDtos = paginatedReviews.Select(r => new ApprovedReviewDto
        {
            Id = r.Id,
            ReviewerName = r.ReviewerName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();

        return new GetApprovedReviewsResponse
        {
            Reviews = reviewDtos,
            TotalCount = totalCount,
            Skip = request.Skip,
            Take = paginatedReviews.Count
        };
    }
}
