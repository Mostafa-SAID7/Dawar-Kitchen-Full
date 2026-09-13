namespace NaarNoor.Application.Common.Interfaces;

/// <summary>
/// Service for generating and validating JWT tokens
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate a JWT token for the given user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email</param>
    /// <param name="roles">User roles</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(string userId, string email, string[] roles);
}
