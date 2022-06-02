using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Trakx.Utils.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

[Collection(nameof(ApiTestCollection))]
public class FireblocksClientTestsBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected ILogger Logger;

    protected FireblocksClientTestsBase(FireblocksApiFixture apiFixture, ITestOutputHelper output)
    {
        Logger = new LoggerConfiguration().WriteTo.TestOutput(output).CreateLogger();

        ServiceProvider = apiFixture.ServiceProvider;
    }
}

[CollectionDefinition(nameof(ApiTestCollection))]
public class ApiTestCollection : ICollectionFixture<FireblocksApiFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class FireblocksApiFixture : IDisposable
{
    public readonly ServiceProvider ServiceProvider;

    public FireblocksApiFixture()
    {
        var configuration = ConfigurationHelper.GetConfigurationFromAws<FireblocksApiConfiguration>()
            with
            {
                BaseUrl = "https://api.fireblocks.io/v1",
            };

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddFireblocksClient(configuration);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        ServiceProvider.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
