using Trakx.Common.Attributes;

namespace Trakx.Fireblocks.ApiClient;

/// <summary>
/// Configuration for the Fireblocks API client.
/// </summary>
public record FireblocksApiConfiguration : FireblocksApiCredentialsConfiguration
{
    /// <summary>
    /// The base URL for the Fireblocks API.
    /// </summary>
    [AwsParameter(AllowGlobal = true)]
    public Uri BaseUrl { get; init; } = new("about:blank");

    /// <inheritdoc />
    [AwsParameter]
    public override string ApiPubKey { get; init; } = default!;

    /// <inheritdoc />
    [AwsParameter]
    public override string ApiPrivateKey { get; init; } = default!;

    /// <summary>
    /// Initial delay used to wait after a failure before retrying.
    /// See <see cref="Polly.Contrib.WaitAndRetry.Backoff"/>.
    /// </summary>
    public int InitialRetryDelayInMilliseconds { get; init; } = 100;

    /// <summary>
    /// The maximum number of failed attempts to query the API
    /// before deciding that a request has failed.
    /// </summary>
    public int MaxRetryCount { get; init; } = 10;
}