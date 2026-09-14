using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Exceptions;

namespace NaarNoor.Application.Features.Orders.Commands.ConfirmOrder;

/// <summary>
/// Handler for ConfirmOrderCommand.
/// Transitions order from Pending to Confirmed state.
/// </summary>
public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new OrderDomainException($"Order with ID {request.OrderId} not found.");

        // Confirm order (will throw if invalid state)
        order.Confirm();

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
