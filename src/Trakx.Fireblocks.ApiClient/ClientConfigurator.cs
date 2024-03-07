using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Configuration for the API client.
/// </summary>
public interface IClientConfigurator
{
    /// <summary>
    /// Configuration for the API client.
    /// </summary>
    FireblocksApiConfiguration ApiClientConfiguration { get; }

    /// <summary>
    /// Credential provider for the API client.
    /// </summary>
    IFireblocksCredentialsProvider CredentialProvider { get; }

    /// <summary>
    /// Factory for <see cref="HttpClient"/>.
    /// </summary>
    IHttpClientFactory HttpClientFactory { get; }
}

/// <inheritdoc cref="IClientConfigurator" />
public class ClientConfigurator : IClientConfigurator
{
    /// <inheritdoc cref="ClientConfigurator" />
    public ClientConfigurator(
        FireblocksApiConfiguration apiClientConfiguration,
        IFireblocksCredentialsProvider credentialProvider,
        IHttpClientFactory httpClientFactory)
    {
        ApiClientConfiguration = apiClientConfiguration;
        CredentialProvider = credentialProvider;
        HttpClientFactory = httpClientFactory;
    }

    /// <inheritdoc />
    public FireblocksApiConfiguration ApiClientConfiguration { get; }

    /// <inheritdoc />
    public IFireblocksCredentialsProvider CredentialProvider { get; }

    /// <inheritdoc />
    public IHttpClientFactory HttpClientFactory { get; }
}