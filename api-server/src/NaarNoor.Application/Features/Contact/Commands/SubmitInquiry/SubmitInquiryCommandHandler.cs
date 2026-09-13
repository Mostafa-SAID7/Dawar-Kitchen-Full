using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Contact.Commands.SubmitInquiry;

public class SubmitInquiryCommandHandler : IRequestHandler<SubmitInquiryCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public SubmitInquiryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(SubmitInquiryCommand request, CancellationToken cancellationToken)
    {
        var inquiry = new ContactInquiry
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Subject = request.Subject,
            Message = request.Message
        };

        _unitOfWork.ContactInquiries.Add(inquiry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return inquiry.Id;
    }
}
