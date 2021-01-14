using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient
{
    internal abstract class AuthorisedClient
    {
        protected readonly ICredentialsProvider CredentialProvider;
        protected string BaseUrl { get; }

        protected AuthorisedClient(ClientConfigurator clientConfigurator)
        {
            BaseUrl = clientConfigurator.ApiConfiguration.BaseUrl;
            CredentialProvider = clientConfigurator.GetCredentialProvider(GetType());
        }
    }
}