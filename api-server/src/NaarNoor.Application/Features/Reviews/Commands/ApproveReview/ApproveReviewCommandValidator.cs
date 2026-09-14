using FluentValidation;

namespace NaarNoor.Application.Features.Reviews.Commands.ApproveReview;

public class ApproveReviewCommandValidator : AbstractValidator<ApproveReviewCommand>
{
    public ApproveReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("Review ID is required.");
    }
}
