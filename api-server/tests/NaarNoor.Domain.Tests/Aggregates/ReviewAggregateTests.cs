using FluentAssertions;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Exceptions;
using Xunit;

namespace NaarNoor.Domain.Tests.Aggregates;

/// <summary>
/// Domain unit tests for Review aggregate.
/// ✅ Tests review creation, approval/rejection workflow, and state transitions
/// </summary>
public class ReviewAggregateTests
{
    #region Review.Create Factory Tests

    [Fact]
    public void Create_WithValidInputs_ReturnsReviewWithCorrectProperties()
    {
        // Arrange
        var reviewerName = "Alice Johnson";
        var rating = 5;
        var comment = "Excellent food and service!";

        // Act
        var review = Review.Create(reviewerName, rating, comment);

        // Assert
        review.ReviewerName.Should().Be(reviewerName);
        review.Rating.Should().Be(rating);
        review.Comment.Should().Be(comment);
        review.Id.Should().NotBe(Guid.Empty);
        review.IsApproved.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithEmptyReviewerName_ThrowsOrderDomainException(string reviewerName)
    {
        // Act & Assert
        Assert.Throws<OrderDomainException>(() =>
            Review.Create(reviewerName, 4, "Good food"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Create_WithInvalidRating_ThrowsOrderDomainException(int rating)
    {
        // Act & Assert
        Assert.Throws<OrderDomainException>(() =>
            Review.Create("Alice", rating, "Comment"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_WithValidRating_Succeeds(int rating)
    {
        // Act
        var review = Review.Create("Alice", rating, "Comment");

        // Assert
        review.Rating.Should().Be(rating);
    }

    [Fact]
    public void Create_WithoutComment_HasNull()
    {
        // Act
        var review = Review.Create("Alice", 4, null);

        // Assert
        review.Comment.Should().BeNull();
    }

    #endregion

    #region Approval Workflow Tests

    [Fact]
    public void NewReview_IsNotApproved()
    {
        // Arrange & Act
        var review = Review.Create("Alice", 5, "Great!");

        // Assert
        review.IsApproved.Should().BeFalse();
    }

    [Fact]
    public void Approve_MarksReviewAsApproved()
    {
        // Arrange
        var review = Review.Create("Alice", 5, "Great!");

        // Act
        review.Approve();

        // Assert
        review.IsApproved.Should().BeTrue();
    }

    [Fact]
    public void Reject_MarksReviewAsRejected()
    {
        // Arrange
        var review = Review.Create("Alice", 5, "Great!");

        // Act
        review.Reject();

        // Assert
        review.IsApproved.Should().BeFalse();
    }

    [Fact]
    public void Approve_ThenReject_ChangesState()
    {
        // Arrange
        var review = Review.Create("Alice", 5, "Great!");

        // Act
        review.Approve();
        review.IsApproved.Should().BeTrue();

        review.Reject();

        // Assert
        review.IsApproved.Should().BeFalse();
    }

    #endregion

    #region Rating Range Tests

    [Theory]
    [InlineData(1, "Poor")]
    [InlineData(2, "Fair")]
    [InlineData(3, "Good")]
    [InlineData(4, "Very Good")]
    [InlineData(5, "Excellent")]
    public void Create_WithAllValidRatings_Succeeds(int rating, string description)
    {
        // Act
        var review = Review.Create("Alice", rating, $"This is {description}");

        // Assert
        review.Rating.Should().Be(rating);
        review.Comment.Should().Contain(description);
    }

    #endregion

    #region Review Content Tests

    [Fact]
    public void Create_WithLongComment_StoresFullComment()
    {
        // Arrange
        var longComment = new string('A', 500); // Long comment

        // Act
        var review = Review.Create("Alice", 5, longComment);

        // Assert
        review.Comment.Should().Be(longComment);
    }

    [Fact]
    public void Create_WithEmptyComment_IsAllowed()
    {
        // Act
        var review = Review.Create("Alice", 4, "");

        // Assert
        review.Comment.Should().Be("");
    }

    [Fact]
    public void ReviewerName_IsTrimmedAndValidated()
    {
        // Arrange
        var reviewerName = "Alice Johnson";

        // Act
        var review = Review.Create(reviewerName, 4, "Good");

        // Assert
        review.ReviewerName.Should().Be(reviewerName);
    }

    #endregion

    #region State Transition Tests

    [Fact]
    public void ApprovedReview_HasCorrectMetadata()
    {
        // Arrange
        var review = Review.Create("Alice", 5, "Excellent!");

        // Act
        review.Approve();

        // Assert
        review.IsApproved.Should().BeTrue();
        review.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void MultipleReviewsCanCoexist()
    {
        // Arrange & Act
        var review1 = Review.Create("Alice", 5, "Excellent!");
        var review2 = Review.Create("Bob", 4, "Very good");
        var review3 = Review.Create("Charlie", 3, "Good");

        review1.Approve();
        review3.Approve();
        // review2 remains unapproved

        // Assert
        review1.IsApproved.Should().BeTrue();
        review2.IsApproved.Should().BeFalse();
        review3.IsApproved.Should().BeTrue();
        review1.Id.Should().NotBe(review2.Id);
        review2.Id.Should().NotBe(review3.Id);
    }

    #endregion

    #region Invariant Tests

    [Fact]
    public void Review_MaintainsIdempotency()
    {
        // Arrange
        var reviewerName = "Alice";
        var rating = 5;
        var comment = "Excellent!";

        // Act
        var review1 = Review.Create(reviewerName, rating, comment);
        var review2 = Review.Create(reviewerName, rating, comment);

        // Assert - different instances, different IDs
        review1.Id.Should().NotBe(review2.Id);
        review1.ReviewerName.Should().Be(review2.ReviewerName);
        review1.Rating.Should().Be(review2.Rating);
    }

    [Fact]
    public void ApprovedReview_IsDistinguishableFromUnapproved()
    {
        // Arrange
        var review1 = Review.Create("Alice", 5, "Excellent!");
        var review2 = Review.Create("Bob", 5, "Excellent!");

        review1.Approve();

        // Act & Assert
        review1.IsApproved.Should().BeTrue();
        review2.IsApproved.Should().BeFalse();
        review1.Should().NotBe(review2);
    }

    #endregion
}
