namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Fireblocks API credentials.
/// </summary>
public record FireblocksApiCredentialsConfiguration
{
    /// <summary>
    /// The public key for the Fireblocks API.
    /// </summary>
    public virtual string ApiPubKey { get; init; } = null!;

    /// <summary>
    /// The private key for the Fireblocks API.
    /// </summary>
    public virtual string ApiPrivateKey { get; init; } = null!;
}