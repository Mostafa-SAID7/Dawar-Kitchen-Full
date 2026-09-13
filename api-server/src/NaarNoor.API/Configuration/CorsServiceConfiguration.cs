namespace NaarNoor.API.Configuration;

using Microsoft.Extensions.Configuration;

/// <summary>
/// CORS service registration - Environment-aware configuration
/// Development: AllowAnyOrigin (localhost development)
/// Production: Specific origins only (CORS_ALLOWED_ORIGINS env var)
/// </summary>
public static class CorsServiceConfiguration
{
    public static void AddCorsServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var corsOriginsEnv = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")
                    ?? configuration["CORS:AllowedOrigins"];

                if (!string.IsNullOrWhiteSpace(corsOriginsEnv))
                {
                    // Explicit origins configured — use them (supports Replit .replit.app domains)
                    policy.WithOrigins(corsOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries))
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
                else if (environment == "Development")
                {
                    // Development fallback: allow any origin so the Angular dev server proxy works
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                }
                else
                {
                    // Production fallback: Dawar Kitchen frontend domain
                    policy.WithOrigins("https://dawar-kitchen.vercel.app", "http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
            });
        });
    }
}
