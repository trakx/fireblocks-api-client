using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient
{
    internal abstract class AuthorisedClient
    {
        protected readonly ICredentialsProvider CredentialProvider;

        protected AuthorisedClient(ClientConfigurator clientConfigurator)
        {
            CredentialProvider = clientConfigurator.CredentialsProvider;
        }
    }
}