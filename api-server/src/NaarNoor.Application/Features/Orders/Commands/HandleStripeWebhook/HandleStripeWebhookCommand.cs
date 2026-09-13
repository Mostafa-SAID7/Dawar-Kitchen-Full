using MediatR;

namespace NaarNoor.Application.Features.Orders.Commands.HandleStripeWebhook;

public record HandleStripeWebhookCommand(
    string Payload,
    string StripeSignatureHeader
) : IRequest<Unit>;

