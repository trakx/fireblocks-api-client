using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Extension method to add the health check for the Fireblocks API.
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    /// Add the health check for the Fireblocks API.
    /// </summary>
    public static IHealthChecksBuilder AddFireblocksApiClientHealthCheck(this IHealthChecksBuilder builder)
    {
        return builder
            .AddCheck<FireblocksApiHealthCheck>("Fireblocks API", tags: ["fireblocks-api-client"]);
    }
}