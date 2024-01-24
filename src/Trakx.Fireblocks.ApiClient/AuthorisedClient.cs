using Trakx.Common.ApiClient;

namespace Trakx.Fireblocks.ApiClient;

internal abstract class AuthorisedClient
{
    public FireblocksApiConfiguration Configuration { get; protected set; }

    protected string _baseUrl ;
    protected Uri BaseUrl => new Uri(_baseUrl) ?? Configuration.BaseUrl;

    protected readonly ICredentialsProvider CredentialProvider;

    protected AuthorisedClient(ClientConfigurator clientConfigurator)
    {
        Configuration = clientConfigurator.Configuration;
        CredentialProvider = clientConfigurator.GetCredentialsProvider();
    }
}