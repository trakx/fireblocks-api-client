using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

public class AddFireblocksClientExtensionTests : CredentialsTestsBase
{
    public AddFireblocksClientExtensionTests(RsaKeyFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    { }

    [Fact]
    public void Services_should_be_built()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFireblocksClient(Configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<IExchange_accountsClient>();
        client.Should().NotBeNull();
    }
}
