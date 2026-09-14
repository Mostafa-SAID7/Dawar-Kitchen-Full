using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.DTOs.MenuItems;
using NaarNoor.Application.DTOs.Orders;
using NaarNoor.Application.Features.MenuItems.Commands.CreateMenuItem;
using NaarNoor.Application.Features.MenuItems.Commands.DeleteMenuItem;
using NaarNoor.Application.Features.MenuItems.Commands.UpdateMenuItem;
using NaarNoor.Application.Features.MenuItems.Queries.GetMenuItemById;
using NaarNoor.Application.Features.MenuItems.Queries.GetMenuItems;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MenuItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? category = null, CancellationToken cancellationToken = default)
    {
        var items = await _mediator.Send(new GetMenuItemsQuery(category), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MenuItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetMenuItemByIdQuery(id), cancellationToken);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemRequest body, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var command = new CreateMenuItemCommand(
            Name:        body.Name ?? body.NameEn ?? "Unnamed",
            Description: body.Description ?? body.DescriptionEn,
            Price:       body.Price,
            Category:    body.Category,
            IsVegetarian: body.IsVegetarian,
            IsVegan:     body.IsVegan,
            IsGlutenFree: body.IsGlutenFree,
            IsAvailable: body.IsAvailable,
            ImageUrl:    body.ImageUrl,
            SortOrder:   body.SortOrder
        );

        var id = await _mediator.Send(command, cancellationToken);
        var created = await _mediator.Send(new GetMenuItemByIdQuery(id), cancellationToken);
        return Created($"/api/menu/{id}", created);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateMenuItemCommand(
            Id:          id,
            Name:        body.Name ?? body.NameEn,
            Description: body.Description ?? body.DescriptionEn,
            Price:       body.Price,
            Category:    body.Category,
            IsVegetarian: body.IsVegetarian,
            IsVegan:     body.IsVegan,
            IsGlutenFree: body.IsGlutenFree,
            IsAvailable: body.IsAvailable,
            ImageUrl:    body.ImageUrl
        );

        var updated = await _mediator.Send(command, cancellationToken);
        if (!updated) return NotFound();

        var item = await _mediator.Send(new GetMenuItemByIdQuery(id), cancellationToken);
        return Ok(item);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteMenuItemCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

}


