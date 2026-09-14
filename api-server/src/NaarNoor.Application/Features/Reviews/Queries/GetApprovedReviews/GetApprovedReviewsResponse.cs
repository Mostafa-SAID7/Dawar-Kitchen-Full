namespace NaarNoor.Application.Features.Reviews.Queries.GetApprovedReviews;

/// <summary>
/// Response DTO for approved reviews query.
/// Contains paginated list of approved reviews.
/// </summary>
public class GetApprovedReviewsResponse
{
    /// <summary>
    /// List of approved review DTOs.
    /// </summary>
    public IReadOnlyList<ApprovedReviewDto> Reviews { get; init; } = new List<ApprovedReviewDto>();

    /// <summary>
    /// Total count of approved reviews in the system.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Number of reviews skipped in this query.
    /// </summary>
    public int Skip { get; init; }

    /// <summary>
    /// Number of reviews returned in this query.
    /// </summary>
    public int Take { get; init; }
}

/// <summary>
/// DTO representing a single approved review.
/// </summary>
public class ApprovedReviewDto
{
    /// <summary>
    /// Review ID.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Reviewer's name.
    /// </summary>
    public required string ReviewerName { get; init; }

    /// <summary>
    /// Star rating (1-5).
    /// </summary>
    public required int Rating { get; init; }

    /// <summary>
    /// Review comment (if any).
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    /// When the review was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}
