using Trakx.Common.ApiClient;

namespace Trakx.Fireblocks.ApiClient;

internal abstract class AuthorisedClient
{
    public FireblocksApiConfiguration Configuration { get; protected set; }
    internal string BaseUrl { get; }
    protected readonly ICredentialsProvider CredentialProvider;

    protected AuthorisedClient(ClientConfigurator clientConfigurator)
    {
        Configuration = clientConfigurator.Configuration;
        CredentialProvider = clientConfigurator.GetCredentialsProvider();

        BaseUrl = Configuration.BaseUrl.AbsoluteUri;
        if (BaseUrl[^1] != '/') BaseUrl += "/";
    }
}