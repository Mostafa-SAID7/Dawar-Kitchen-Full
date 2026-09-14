using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Features.Reviews.Commands.CreateReview;

/// <summary>
/// Handler for CreateReviewCommand.
/// Creates a new review using the Review aggregate factory method.
/// ✅ Uses Review.Create() for domain validation
/// ✅ Single SaveChangesAsync (atomic)
/// ✅ No EF Core in Application layer
/// </summary>
public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // ✅ Use Review aggregate factory method for domain validation
        var review = Review.Create(
            reviewerName: request.ReviewerName,
            rating: request.Rating,
            comment: request.Comment
        );

        // Add to repository
        _unitOfWork.Reviews.Add(review);

        // ✅ Single SaveChangesAsync — atomic operation
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}
