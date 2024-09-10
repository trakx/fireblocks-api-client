using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Fireblocks API health check.
/// </summary>
public class FireblocksApiHealthCheck : IHealthCheck
{
    private const string ErrorMessage = "Unable to test Fireblocks API.";
    private readonly IBlockchains_and_assetsClient _apiClient;

    /// <inheritdoc cref="FireblocksApiHealthCheck" />
    public FireblocksApiHealthCheck(IBlockchains_and_assetsClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = await _apiClient.GetSupportedAssetsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ErrorMessage, ex);
        }
        return HealthCheckResult.Healthy();
    }
}