using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient;

internal abstract class AuthorisedClient
{
#nullable disable
    public FireblocksApiConfiguration Configuration { get; protected set; }
#nullable restore

    private string? _baseUrl;
    protected string BaseUrl => _baseUrl ??= Configuration!.BaseUrl;

    protected readonly ICredentialsProvider CredentialProvider;

    protected AuthorisedClient(ClientConfigurator clientConfigurator)
    {
        Configuration = clientConfigurator.Configuration;
        CredentialProvider = clientConfigurator.GetCredentialsProvider();
    }
}
