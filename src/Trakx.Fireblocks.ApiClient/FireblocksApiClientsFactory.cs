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
public class FireblocksApiClientsFactory : IFireblocksApiClientsFactory
{
    private readonly FireblocksApiConfiguration _apiConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <inheritdoc cref="FireblocksApiClientsFactory" />
    public FireblocksApiClientsFactory(
        FireblocksApiConfiguration apiConfiguration,
        IHttpClientFactory httpClientFactory,
        IDateTimeProvider dateTimeProvider)
    {
        _apiConfiguration = apiConfiguration;
        _httpClientFactory = httpClientFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public TApiClient CreateApiClient<TApiClient>(FireblocksApiCredentialsConfiguration credentialsConfiguration) where TApiClient : IFireblocksApiClientBase
    {
        var bearerCredentialsProvider = new BearerCredentialsProvider(credentialsConfiguration, _dateTimeProvider);
        var apiCredentialsProvider = new ApiKeyCredentialsProvider(credentialsConfiguration, bearerCredentialsProvider);
        var clientConfigurator = new ClientConfigurator(_apiConfiguration, apiCredentialsProvider, _httpClientFactory);

        var implementationType = ApiClientReflection.GetApiClientImplementation(typeof(TApiClient));
        if (implementationType is null)
            throw new NotImplementedException($"No implementation found for {typeof(TApiClient).Name}");

        var client = (TApiClient)Activator.CreateInstance(implementationType, clientConfigurator)!;
        return client;
    }
}