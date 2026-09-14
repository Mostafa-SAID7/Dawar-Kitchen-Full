using NaarNoor.Domain.Common;
using NaarNoor.Domain.Exceptions;

namespace NaarNoor.Domain.Entities;

/// <summary>
/// Review aggregate representing a customer's feedback on menu items or dining experience.
/// Enforces business rules for ratings, reviewer names, and approval workflows.
/// </summary>
public class Review : BaseEntity
{
    private const int MinRating = 1;
    private const int MaxRating = 5;

    public string ReviewerName { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public bool IsApproved { get; set; } = false;

    /// <summary>
    /// Factory method for creating a new Review.
    /// Validates all required fields and business rules.
    /// </summary>
    /// <exception cref="OrderDomainException">Thrown when review data is invalid.</exception>
    public static Review Create(
        string reviewerName,
        int rating,
        string? comment = null)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(reviewerName))
            throw new OrderDomainException("Reviewer name is required.");

        // Validate rating range
        if (rating < MinRating || rating > MaxRating)
            throw new OrderDomainException($"Rating must be between {MinRating} and {MaxRating} stars.");

        return new Review
        {
            ReviewerName = reviewerName.Trim(),
            Rating = rating,
            Comment = comment?.Trim(),
            IsApproved = false // New reviews start unapproved
        };
    }

    /// <summary>
    /// Approves the review for public display.
    /// </summary>
    public void Approve()
    {
        IsApproved = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejects the review (sets approved to false).
    /// </summary>
    public void Reject()
    {
        IsApproved = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the review satisfies all business rule invariants.
    /// </summary>
    public bool IsInValidState()
    {
        return !string.IsNullOrWhiteSpace(ReviewerName)
            && Rating >= MinRating
            && Rating <= MaxRating;
    }
}

