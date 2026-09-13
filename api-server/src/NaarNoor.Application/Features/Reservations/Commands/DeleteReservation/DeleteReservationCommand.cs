using MediatR;

namespace NaarNoor.Application.Features.Reservations.Commands.DeleteReservation;

public record DeleteReservationCommand(Guid Id) : IRequest<bool>;

