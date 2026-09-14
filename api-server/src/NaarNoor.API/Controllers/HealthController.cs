using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.API.Controllers;

/// <summary>
/// Health check endpoint for service availability monitoring
/// ✅ FIXED: Now uses IUnitOfWork instead of direct ApplicationDbContext
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public HealthController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
            // Test database connection by querying through repository
            var canConnect = await _unitOfWork.Users.GetAllAsync();
            var databaseStatus = canConnect.Any() ? "Connected" : "Reachable (no users)";

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
