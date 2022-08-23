using Trakx.Fireblocks.ApiClient.Utils;
using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient;

internal class ClientConfigurator
{
    private readonly IFireblocksCredentialsProvider _provider;

    public ClientConfigurator(FireblocksApiConfiguration configuration,
        IFireblocksCredentialsProvider provider)
    {
        _provider = provider;
        Configuration = configuration;
    }

    public FireblocksApiConfiguration Configuration { get; }

    public ICredentialsProvider GetCredentialsProvider()
    {
        return _provider;
    }
}
