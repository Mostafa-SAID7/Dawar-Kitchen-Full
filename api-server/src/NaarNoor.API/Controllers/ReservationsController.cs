using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.DTOs;
using NaarNoor.Application.DTOs.Reservations;
using NaarNoor.Application.Features.Reservations.Commands.CreateReservation;
using NaarNoor.Application.Features.Reservations.Commands.DeleteReservation;
using NaarNoor.Application.Features.Reservations.Commands.UpdateReservation;
using NaarNoor.Application.Features.Reservations.Queries.GetReservationById;
using NaarNoor.Application.Features.Reservations.Queries.GetReservations;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateReservationBodyRequest body, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var command = new CreateReservationCommand(
            CustomerName:    body.CustomerName,
            Email:           body.CustomerEmail ?? body.Email ?? "guest@naarnoor.com",
            PhoneNumber:     body.CustomerPhone ?? body.PhoneNumber ?? "",
            BookingTime:     body.BookingTime,
            ReservationDate: body.ReservationDate,
            ReservationTime: body.ReservationTime,
            PartySize:       body.PartySize,
            SpecialRequests: body.SpecialRequests
        );

        var id = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, new CreateReservationResponse { Id = id.ToString() });
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetReservationsQuery(page, pageSize);
        var reservations = await _mediator.Send(query, cancellationToken);
        return Ok(reservations);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetReservationByIdQuery(id);
        var reservation = await _mediator.Send(query, cancellationToken);
        
        if (reservation is null) return NotFound();
        return Ok(reservation);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReservationBodyRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateReservationCommand(
            Id:             id,
            CustomerName:   body.CustomerName,
            Email:          body.CustomerEmail ?? body.Email,
            PhoneNumber:    body.CustomerPhone ?? body.PhoneNumber,
            Status:         body.Status,
            PartySize:      body.PartySize,
            SpecialRequests: body.SpecialRequests
        );

        var updated = await _mediator.Send(command, cancellationToken);
        if (!updated) return NotFound();

        return await GetById(id, cancellationToken);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteReservationCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

public class CreateReservationResponse
{
    public string Id { get; set; } = string.Empty;
}

