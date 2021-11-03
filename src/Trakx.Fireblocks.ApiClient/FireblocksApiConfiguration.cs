using Trakx.Utils.Attributes;

namespace Trakx.Fireblocks.ApiClient
{
    public record FireblocksApiConfiguration
    {
        
        public string BaseUrl { get; set; }
        
#nullable disable
        [SecretEnvironmentVariable]
        public string ApiPubKey { get; init; }

        [SecretEnvironmentVariable]
        public string ApiPrivateKey { get; init; }
#nullable restore
    }
}