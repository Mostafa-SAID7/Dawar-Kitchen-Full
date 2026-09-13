using FluentValidation;

namespace NaarNoor.Application.Reservations.Commands.CreateReservation;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20);

        RuleFor(x => x.ReservationDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Reservation date must be today or in the future.");

        RuleFor(x => x.ReservationTime)
            .NotEmpty().WithMessage("Reservation time is required.");

        RuleFor(x => x.PartySize)
            .InclusiveBetween(1, 20)
            .WithMessage("Party size must be between 1 and 20.");
    }
}
