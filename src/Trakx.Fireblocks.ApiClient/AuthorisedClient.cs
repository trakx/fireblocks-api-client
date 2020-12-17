using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient
{
    internal abstract class AuthorisedClient
    {
        protected readonly ICredentialsProvider CredentialProvider;
        protected string BaseUrl { get; }

        protected AuthorisedClient(ClientConfigurator clientConfigurator) : base(clientConfigurator)
        {
            CredentialProvider = clientConfigurator.GetCredentialProvider(GetType());
            BaseUrl = clientConfigurator.ApiConfiguration.BaseUrl;
        }
    }
}