using MediatR;

namespace NaarNoor.Application.Features.Reviews.Queries.GetApprovedReviews;

/// <summary>
/// Query to retrieve all approved reviews for public display.
/// Supports optional pagination (skip/take).
/// </summary>
public class GetApprovedReviewsQuery : IRequest<GetApprovedReviewsResponse>
{
    /// <summary>
    /// Number of reviews to skip (for pagination). Default: 0.
    /// </summary>
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Maximum number of reviews to return. Default: 10. Max: 100.
    /// </summary>
    public int Take { get; init; } = 10;
}
