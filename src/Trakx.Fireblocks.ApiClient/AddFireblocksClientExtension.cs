using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trakx.Utils.DateTimeHelpers;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient;

public static partial class AddFireblocksClientExtension
{
    public static IServiceCollection AddFireblocksClient(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<FireblocksApiConfiguration>(
            configuration.GetSection(nameof(FireblocksApiConfiguration)));
        AddCommonDependencies(services);

        return services;
    }

    public static IServiceCollection AddFireblocksClient(
        this IServiceCollection services, FireblocksApiConfiguration apiConfiguration)
    {
        var options = Options.Create(apiConfiguration);
        services.AddSingleton(options);

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
