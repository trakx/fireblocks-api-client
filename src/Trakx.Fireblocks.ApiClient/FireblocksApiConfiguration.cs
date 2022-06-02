using Trakx.Utils.Attributes;

namespace Trakx.Fireblocks.ApiClient;

public record FireblocksApiConfiguration
{
#nullable disable
    public string BaseUrl { get; set; }

    [AwsParameter, SecretEnvironmentVariable]
    public string ApiPubKey { get; init; }

    [AwsParameter, SecretEnvironmentVariable]
    public string ApiPrivateKey { get; init; }
#nullable restore
}
