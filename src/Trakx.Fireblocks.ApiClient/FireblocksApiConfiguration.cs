using Trakx.Utils.Attributes;

namespace Trakx.Fireblocks.ApiClient;

public record FireblocksApiConfiguration
{
    public string BaseUrl { get; set; }

#nullable disable
    [AwsParameter, SecretEnvironmentVariable]
    public string ApiPubKey { get; init; }

    [AwsParameter, SecretEnvironmentVariable]
    public string ApiPrivateKey { get; init; }
#nullable restore
}
