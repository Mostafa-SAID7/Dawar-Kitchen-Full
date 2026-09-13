using MediatR;

namespace NaarNoor.Application.Features.Contact.Commands.SubmitInquiry;

public record SubmitInquiryCommand(
    string Name,
    string Email,
    string? PhoneNumber,
    string Subject,
    string Message
) : IRequest<Guid>;

