using Trakx.Common.ApiClient;
using Trakx.Fireblocks.ApiClient.Utils;

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
