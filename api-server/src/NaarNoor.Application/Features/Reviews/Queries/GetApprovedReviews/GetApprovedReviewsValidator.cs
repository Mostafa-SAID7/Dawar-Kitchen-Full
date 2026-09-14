using FluentValidation;

namespace NaarNoor.Application.Features.Reviews.Queries.GetApprovedReviews;

public class GetApprovedReviewsValidator : AbstractValidator<GetApprovedReviewsQuery>
{
    public GetApprovedReviewsValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0).WithMessage("Skip must be >= 0.");

        RuleFor(x => x.Take)
            .GreaterThan(0).WithMessage("Take must be > 0.")
            .LessThanOrEqualTo(100).WithMessage("Take must be <= 100 (max 100 reviews per request).");
    }
}
