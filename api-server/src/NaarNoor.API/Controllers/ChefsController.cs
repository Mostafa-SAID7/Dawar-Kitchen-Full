using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Features.Chefs.Queries.GetChefs;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.DTOs;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChefsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public ChefsController(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
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
        var chef = await _unitOfWork.Chefs.Query()
            .Where(c => c.Id == id && c.IsActive)
            .Select(c => new ChefDto
            {
                Id = c.Id,
                Name = c.Name,
                Title = c.Title,
                Bio = c.Bio,
                ImageUrl = c.ImageUrl,
                Specialty = c.Specialty,
                SortOrder = c.SortOrder
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (chef is null) return NotFound();
        return Ok(chef);
    }
}

