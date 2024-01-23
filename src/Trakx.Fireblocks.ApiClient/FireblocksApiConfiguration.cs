using Trakx.Common.Attributes;

namespace Trakx.Fireblocks.ApiClient;

public record FireblocksApiConfiguration
{
#nullable disable
    
    [AwsParameter(AllowGlobal = true)]
    public Uri BaseUrl { get; init; } = new("about:blank");

    [AwsParameter]
    public string ApiPubKey { get; init; }

    [AwsParameter]
    public string ApiPrivateKey { get; init; }
#nullable restore
}
