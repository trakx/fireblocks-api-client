using Trakx.Common.Attributes;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Configuration for the Fireblocks API client.
/// </summary>
public record FireblocksApiConfiguration
{
    /// <summary>
    /// The base URL for the Fireblocks API.
    /// </summary>
    [AwsParameter(AllowGlobal = true)]
    public Uri BaseUrl { get; init; } = new("about:blank");

    /// <summary>
    /// The public key for the Fireblocks API.
    /// </summary>
    [AwsParameter]
    public string ApiPubKey { get; init; } = default!;

    /// <summary>
    /// The private key for the Fireblocks API.
    /// </summary>
    [AwsParameter]
    public string ApiPrivateKey { get; init; } = default!;
}