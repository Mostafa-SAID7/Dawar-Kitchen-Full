using MediatR;

namespace NaarNoor.Application.Features.Reviews.Commands.CreateReview;

/// <summary>
/// Command to create a new review for menu items or dining experience.
/// Represents a customer's feedback with rating (1-5) and optional comment.
/// Reviews start in an unapproved state pending moderation.
/// </summary>
public class CreateReviewCommand : IRequest<Guid>
{
    /// <summary>
    /// Name of the reviewer.
    /// </summary>
    public required string ReviewerName { get; init; }

    /// <summary>
    /// Rating in stars (1-5).
    /// </summary>
    public required int Rating { get; init; }

    /// <summary>
    /// Optional detailed review comment.
    /// </summary>
    public string? Comment { get; init; }
}
