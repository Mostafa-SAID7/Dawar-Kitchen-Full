using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Features.Reviews.Commands.ApproveReview;
using NaarNoor.Application.Features.Reviews.Commands.CreateReview;
using NaarNoor.Application.Features.Reviews.Queries.GetApprovedReviews;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submit a new review for the restaurant.
    /// Reviews are created in unapproved state pending moderation.
    /// </summary>
    [AllowAnonymous]  // ✅ SECURITY: Explicitly allow unauthenticated review submissions
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReview(
        [FromBody] CreateReviewCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, new { id });
    }

    /// <summary>
    /// Get approved reviews for public display.
    /// Supports pagination via skip and take query parameters.
    /// </summary>
    [AllowAnonymous]  // ✅ SECURITY: Public endpoint for approved reviews only
    [HttpGet("approved")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetApprovedReviews(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetApprovedReviewsQuery { Skip = skip, Take = take };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Approve a review for public display.
    /// Restricted to admin/moderator users.
    /// </summary>
    [Authorize(Roles = "Admin,Moderator")]  // ✅ SECURITY: Only admins/moderators can approve
    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ApproveReview(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ApproveReviewCommand { ReviewId = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
