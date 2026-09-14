using MediatR;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.Features.Reviews.Commands.ApproveReview;

/// <summary>
/// Handler for ApproveReviewCommand.
/// Retrieves a review and marks it as approved for public display.
/// ✅ Uses IUnitOfWork for repository access
/// ✅ Single SaveChangesAsync (atomic)
/// </summary>
public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the review
        var review = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new InvalidOperationException($"Review with ID {request.ReviewId} not found.");

        // Use domain method to approve
        review.Approve();

        // Update in repository
        _unitOfWork.Reviews.Update(review);

        // ✅ Single SaveChangesAsync — atomic operation
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
