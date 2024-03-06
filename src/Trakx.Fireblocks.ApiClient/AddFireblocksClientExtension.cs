using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.DateAndTime;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Extension methods for adding the Fireblocks API client to the service collection.
/// </summary>
public static partial class AddFireblocksClientExtension
{
    /// <summary>
    /// Add the Fireblocks API client to the service collection.
    /// </summary>
    public static IServiceCollection AddFireblocksClient(
        this IServiceCollection services, IConfiguration configuration)
    {
        var apiConfiguration = configuration.GetSection(nameof(FireblocksApiConfiguration))
            .Get<FireblocksApiConfiguration>();

        AddFireblocksClient(services, apiConfiguration!);

        return services;
    }

    /// <inheritdoc cref="AddFireblocksClient(IServiceCollection, IConfiguration)" />
    public static IServiceCollection AddFireblocksClient(
        this IServiceCollection services, FireblocksApiConfiguration apiConfiguration)
    {
        services.AddSingleton(apiConfiguration);

        AddCommonDependencies(services);

        return services;
    }

    private static void AddCommonDependencies(IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IBearerCredentialsProvider, BearerCredentialsProvider>();
        services.AddSingleton<IFireblocksCredentialsProvider, ApiKeyCredentialsProvider>();

        services.AddSingleton<ClientConfigurator>();
        AddClients(services);
    }
}