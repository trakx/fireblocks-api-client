using Trakx.Fireblocks.ApiClient.Utils;
using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient
{
    internal class ClientConfigurator
    {
        public ClientConfigurator(IFireblocksCredentialsProvider credentialsProvider)
        {
            CredentialsProvider = credentialsProvider;
        }

        public ICredentialsProvider CredentialsProvider { get; }
    }
}