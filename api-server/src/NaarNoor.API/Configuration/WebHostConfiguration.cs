namespace NaarNoor.API.Configuration;

/// <summary>
/// Web host configuration for port and URL binding
/// Binds to PORT environment variable (default 8080) as per requirement
/// </summary>
public static class WebHostConfiguration
{
    public static void ConfigureWebHost(this WebApplicationBuilder builder)
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    }
}
