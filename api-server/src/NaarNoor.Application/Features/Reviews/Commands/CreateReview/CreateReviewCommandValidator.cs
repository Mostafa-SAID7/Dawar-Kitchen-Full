using FluentValidation;

namespace NaarNoor.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ReviewerName)
            .NotEmpty().WithMessage("Reviewer name is required.")
            .MaximumLength(100).WithMessage("Reviewer name must not exceed 100 characters.");

        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Rating is required.")
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}
