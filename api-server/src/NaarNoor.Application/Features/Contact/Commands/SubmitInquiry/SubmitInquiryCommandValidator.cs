using FluentValidation;

namespace NaarNoor.Application.Contact.Commands.SubmitInquiry;

public class SubmitInquiryCommandValidator : AbstractValidator<SubmitInquiryCommand>
{
    public SubmitInquiryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(200);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(2000);
    }
}
