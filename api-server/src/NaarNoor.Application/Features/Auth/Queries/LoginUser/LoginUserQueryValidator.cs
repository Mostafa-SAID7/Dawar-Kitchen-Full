using FluentValidation;

namespace NaarNoor.Application.Features.Auth.Queries.LoginUser;

/// <summary>
/// Validator for LoginUserQuery
/// </summary>
public class LoginUserQueryValidator : AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}

