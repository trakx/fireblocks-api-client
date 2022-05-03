using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trakx.Fireblocks.ApiClient.Utils;
using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient;

internal class ClientConfigurator
{
    private readonly IServiceProvider _provider;

    public ClientConfigurator(IServiceProvider provider)
    {
        _provider = provider;
        Configuration = provider.GetService<IOptions<FireblocksApiConfiguration>>()!.Value;
    }

    public FireblocksApiConfiguration Configuration { get; }

    public ICredentialsProvider? GetCredentialsProvider()
    {
        return _provider.GetService<IFireblocksCredentialsProvider>();
    }
}