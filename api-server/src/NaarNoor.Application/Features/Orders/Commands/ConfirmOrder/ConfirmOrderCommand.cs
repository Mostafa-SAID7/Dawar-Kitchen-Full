using MediatR;

namespace NaarNoor.Application.Features.Orders.Commands.ConfirmOrder;

/// <summary>
/// Command to confirm a pending order (Pending → Confirmed).
/// ✅ Requires order to have at least one item
/// </summary>
public record ConfirmOrderCommand(Guid OrderId) : IRequest<bool>;
