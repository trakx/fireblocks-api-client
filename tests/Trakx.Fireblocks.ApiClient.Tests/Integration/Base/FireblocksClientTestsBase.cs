using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Trakx.Common.Testing.Configuration;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration.Base;

[Collection(nameof(ApiTestCollection))]
public class FireblocksClientTestsBase
{
    protected readonly ServiceProvider _serviceProvider;
    protected readonly ILogger _logger;

    protected FireblocksClientTestsBase(FireblocksApiFixture apiFixture, ITestOutputHelper output)
    {
        _logger = new LoggerConfiguration().WriteTo.TestOutput(output).CreateLogger();
        _serviceProvider = apiFixture.ServiceProvider;
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
        var apiConfiguration = AwsConfigurationHelper.GetConfigurationFromAws<FireblocksApiConfiguration>()
        with
        {
            BaseUrl = new Uri("https://api.fireblocks.io/v1")
        };

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFireblocksClient(apiConfiguration);

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