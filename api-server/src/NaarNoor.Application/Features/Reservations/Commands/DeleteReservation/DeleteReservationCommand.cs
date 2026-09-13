using MediatR;

namespace NaarNoor.Application.Reservations.Commands.DeleteReservation;

public record DeleteReservationCommand(Guid Id) : IRequest<bool>;
