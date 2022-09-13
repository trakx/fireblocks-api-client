using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Utils.DateTimeHelpers;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient;

public static partial class AddFireblocksClientExtension
{
    public static IServiceCollection AddFireblocksClient(
        this IServiceCollection services, IConfiguration configuration)
    {
        var apiConfiguration = configuration.GetSection(nameof(FireblocksApiConfiguration))
            .Get<FireblocksApiConfiguration>();

        AddFireblocksClient(services, apiConfiguration);

        return services;
    }

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
