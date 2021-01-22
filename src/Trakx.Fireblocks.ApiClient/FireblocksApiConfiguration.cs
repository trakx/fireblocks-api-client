using Trakx.Utils.Attributes;

namespace Trakx.Fireblocks.ApiClient
{
    public record FireblocksApiConfiguration
    {
#nullable disable
        [SecretEnvironmentVariable]
        public string ApiPubKey { get; init; }

        [SecretEnvironmentVariable]
        public string ApiPrivateKey { get; init; }
#nullable restore
    }
}