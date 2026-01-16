using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.ApiClient.Utils;
using Trakx.Common.DateAndTime;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Factory to create Fireblocks API clients instances.
/// </summary>
public interface IFireblocksApiClientsFactory
{
    /// <summary>
    /// Create an instance of an Fireblocks API client.
    /// </summary>
    TApiClient CreateApiClient<TApiClient>(FireblocksApiCredentialsConfiguration credentialsConfiguration) where TApiClient : IFireblocksApiClientBase;
}

/// <inheritdoc />
public class FireblocksApiClientsFactory(
    IServiceProvider serviceProvider,
    IDateTimeProvider dateTimeProvider) : IFireblocksApiClientsFactory
{
    /// <inheritdoc />
    public TApiClient CreateApiClient<TApiClient>(FireblocksApiCredentialsConfiguration credentialsConfiguration) where TApiClient : IFireblocksApiClientBase
    {
        var defaultConfiguration = serviceProvider.GetRequiredService<FireblocksApiConfiguration>();

        var customConfiguration = defaultConfiguration with
        {
            ApiPubKey = credentialsConfiguration.ApiPubKey,
            ApiPrivateKey = credentialsConfiguration.ApiPrivateKey
        };

        var bearerCredentialsProvider = new BearerCredentialsProvider(customConfiguration, dateTimeProvider);
        var apiCredentialsProvider = new ApiKeyCredentialsProvider(credentialsConfiguration, bearerCredentialsProvider);
        var clientConfigurator = new ClientConfigurator(apiCredentialsProvider);

        var implementationType = ApiClientReflection.GetApiClientImplementation(typeof(TApiClient));
        if (implementationType is null)
            throw new NotImplementedException($"No implementation found for {typeof(TApiClient).Name}");

        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient(implementationType.FullName!);

        var client = (TApiClient)ActivatorUtilities.CreateInstance(serviceProvider, implementationType, clientConfigurator, httpClient);
        return client;
    }
}