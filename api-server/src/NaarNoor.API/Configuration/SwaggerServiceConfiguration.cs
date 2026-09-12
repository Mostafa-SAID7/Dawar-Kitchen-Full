using Microsoft.OpenApi.Models;

namespace NaarNoor.API.Configuration;

/// <summary>
/// Swagger/OpenAPI service registration
/// </summary>
public static class SwaggerServiceConfiguration
{
    public static void AddSwaggerServiceConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Register API documentation
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Naar & Noor API",
                Version = "v1",
                Description = "Restaurant management API with menu, chefs, reservations, orders, and contact endpoints",
                Contact = new OpenApiContact
                {
                    Name = "Naar & Noor",
                    Url = new Uri("https://naar-noor.vercel.app")
                }
            });
        });
    }
}
