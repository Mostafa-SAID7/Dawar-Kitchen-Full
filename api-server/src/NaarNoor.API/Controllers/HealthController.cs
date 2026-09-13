using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.API.Controllers;

/// <summary>
/// Health check endpoint for service availability monitoring
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;

    public HealthController(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns the health status of the Dawar Kitchen API
    /// </summary>
    /// <returns>Health status with timestamp and version info</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            // Test database connection
            var canConnect = await _dbContext.Users.AnyAsync();
            var databaseStatus = canConnect ? "Connected" : "Unreachable";

            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString("O"),
                version = "1.0.0",
                database = databaseStatus
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow.ToString("O"),
                version = "1.0.0",
                database = "Disconnected",
                error = ex.Message
            });
        }
    }
}
