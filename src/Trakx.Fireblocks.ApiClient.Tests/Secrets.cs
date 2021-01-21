using Trakx.Utils.Attributes;
using Trakx.Utils.Testing;

namespace Trakx.Fireblocks.ApiClient.Tests
{
    public record Secrets : SecretsBase
    {
        #nullable disable
        [SecretEnvironmentVariable(nameof(FireblocksApiConfiguration), nameof(FireblocksApiConfiguration.ApiPubKey))]
        public string ApiPubKey { get; init; }

        [SecretEnvironmentVariable(nameof(FireblocksApiConfiguration), nameof(FireblocksApiConfiguration.ApiPrivateKey))]
        public string ApiPrivateKey { get; init; }
        #nullable restore
    }
}