using Trakx.Utils.Attributes;

namespace Trakx.Fireblocks.ApiClient
{
    public class FireblocksApiConfiguration
    {
#nullable disable
        [SecretEnvironmentVariable]
        public string ApiPubKey { get; set; }

        [SecretEnvironmentVariable]
        public string ApiPrivateKey { get; set; }
#nullable restore
    }
}