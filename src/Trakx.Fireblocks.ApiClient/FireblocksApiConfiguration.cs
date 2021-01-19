using Trakx.Utils.Attributes;

namespace Trakx.Fireblocks.ApiClient
{
    public class FireblocksApiConfiguration
    {
#nullable disable
        [SecretEnvironmentVariable("FireblocksApiConfiguration__ApiSecret")]
        public string ApiPubKey { get; set; }

        [SecretEnvironmentVariable("FireblocksApiConfiguration__ApiKey")]
        public string ApiPrivateKey { get; set; }
#nullable restore
    }
}