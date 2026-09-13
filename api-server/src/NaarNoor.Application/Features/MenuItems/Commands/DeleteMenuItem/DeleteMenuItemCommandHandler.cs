using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.Application.MenuItems.Commands.DeleteMenuItem;

public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMenuItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.MenuItems.Query()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (item is null) return false;

        _unitOfWork.MenuItems.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
