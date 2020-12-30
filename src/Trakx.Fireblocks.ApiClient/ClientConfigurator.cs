using Trakx.Utils.Api;

namespace Trakx.Fireblocks.ApiClient
{
    internal class ClientConfigurator
    {
        public ClientConfigurator(ICredentialsProvider credentialsProvider)
        {
            CredentialsProvider = credentialsProvider;
        }

        public ICredentialsProvider CredentialsProvider { get; }
    }
}