using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.Services;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Infrastructure.Services;

/// <summary>
/// User service implementation with local password hashing and database persistence
/// ✅ FIXED: Uses IUnitOfWork instead of direct ApplicationDbContext (Clean Architecture)
/// All User CRUD operations now go through the repository abstraction
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user with email and password using local password hash comparison
    /// </summary>
    public async Task<UserAuthResult> AuthenticateAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new UserAuthResult { Success = false, Error = "Email and password are required" };

            // Query user through repository (no direct DbContext)
            var user = await _unitOfWork.Users.FindAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Email}", email);
                return new UserAuthResult { Success = false, Error = "Invalid email or password" };
            }

            // Verify password using PasswordHasher
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Failed password verification for user: {Email}", email);
                return new UserAuthResult { Success = false, Error = "Invalid email or password" };
            }

            _logger.LogInformation("User authenticated successfully: {Email}", email);

            return new UserAuthResult
            {
                Success = true,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = user.Roles,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication failed for {Email}", email);
            return new UserAuthResult { Success = false, Error = "Authentication failed" };
        }
    }

    /// <summary>
    /// Register new user with password hashing
    /// </summary>
    public async Task<UserAuthResult> RegisterAsync(string email, string password, string fullName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return new UserAuthResult { Success = false, Error = "Email is required" };

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return new UserAuthResult { Success = false, Error = "Password must be at least 8 characters" };

            // Check if user already exists (via repository)
            var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt for existing user: {Email}", email);
                return new UserAuthResult { Success = false, Error = "User already exists" };
            }

            // Create new user with hashed password
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                FullName = string.IsNullOrWhiteSpace(fullName) ? email : fullName,
                Roles = new[] { "User" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Hash the password using PasswordHasher
            var hasher = new PasswordHasher<User>();
            newUser.PasswordHash = hasher.HashPassword(newUser, password);

            // Add to repository and save (via UnitOfWork)
            _unitOfWork.Users.Add(newUser);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {Email}", email);

            return new UserAuthResult
            {
                Success = true,
                User = new UserDto
                {
                    Id = newUser.Id,
                    Email = newUser.Email,
                    FullName = newUser.FullName,
                    Roles = newUser.Roles,
                    CreatedAt = newUser.CreatedAt,
                    IsActive = newUser.IsActive
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for {Email}", email);
            return new UserAuthResult { Success = false, Error = "Registration failed" };
        }
    }

    /// <summary>
    /// Get user by ID from database via repository
    /// </summary>
    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// Get user by email from database via repository
    /// </summary>
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Email == email);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user by email");
            return null;
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                _logger.LogWarning("Password change failed for {UserId}: password too short", userId);
                return false;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
            {
                _logger.LogWarning("Password change failed: user not found {UserId}", userId);
                return false;
            }

            // Verify current password using PasswordHasher
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Password change failed for {UserId}: incorrect current password", userId);
                return false;
            }

            // Hash new password
            user.PasswordHash = hasher.HashPassword(user, newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Update via repository and save (via UnitOfWork)
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password change failed for {UserId}", userId);
            return false;
        }
    }
}
