using Trakx.Common.Attributes;

namespace Trakx.Fireblocks.ApiClient;

public record FireblocksApiConfiguration
{
#nullable disable
    public string BaseUrl { get; set; }

    [AwsParameter]
    public string ApiPubKey { get; init; }

    [AwsParameter]
    public string ApiPrivateKey { get; init; }
#nullable restore
}
