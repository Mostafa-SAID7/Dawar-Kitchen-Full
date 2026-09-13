using FluentValidation;

namespace NaarNoor.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Validator for CreateOrderCommand - validates order structure and business rules
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Order type is required.")
            .Must(t => t == "collection" || t == "delivery" || t == "dine-in")
            .WithMessage("Order type must be 'collection', 'delivery', or 'dine-in'.");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage("Delivery address is required for delivery orders.")
            .When(x => x.Type == "delivery");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("Order must contain at least one item.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.MenuItemId)
                    .NotEmpty().WithMessage("Menu item ID is required.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                    .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100 per item.");
            });
    }
}
