using MediatR;

namespace NaarNoor.Application.Features.Reviews.Commands.ApproveReview;

/// <summary>
/// Command to approve a review for public display.
/// Typically restricted to admin/moderator users.
/// </summary>
public class ApproveReviewCommand : IRequest<Unit>
{
    /// <summary>
    /// ID of the review to approve.
    /// </summary>
    public required Guid ReviewId { get; init; }
}
