using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Configuration for the API client.
/// </summary>
public interface IClientConfigurator
{
    /// <summary>
    /// Credential provider for the API client.
    /// </summary>
    IFireblocksCredentialsProvider CredentialProvider { get; }
}

/// <inheritdoc cref="IClientConfigurator" />
public class ClientConfigurator(IFireblocksCredentialsProvider credentialProvider) : IClientConfigurator
{
    /// <inheritdoc />
    public IFireblocksCredentialsProvider CredentialProvider { get; } = credentialProvider;
}