using MediatR;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Features.Chefs.Queries.GetChefs;
using NaarNoor.Application.Features.Chefs.Queries.GetChefById;
using NaarNoor.Application.DTOs;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChefsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChefsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ChefDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var chefs = await _mediator.Send(new GetChefsQuery(), cancellationToken);
        return Ok(chefs);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ChefDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var chef = await _mediator.Send(new GetChefByIdQuery(id), cancellationToken);
        return chef is null ? NotFound() : Ok(chef);
    }
}

