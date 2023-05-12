using Trakx.Common.Attributes;

namespace Trakx.Fireblocks.ApiClient;

public record FireblocksApiConfiguration
{
#nullable disable
    public string BaseUrl { get; set; }

    [AwsParameter]
    public string ApiPubKey { get; set; }

    [AwsParameter]
    public string ApiPrivateKey { get; set; }
#nullable restore
}
