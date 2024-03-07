using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.DateAndTime;
using Trakx.Common.Testing.Configuration;
using Trakx.Common.Testing.Extensions;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

public class AddFireblocksClientExtensionTests : CredentialsTestsBase
{
    private readonly ServiceProvider _serviceProvider;

    public AddFireblocksClientExtensionTests(RsaKeyFixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFireblocksClient(Configuration);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Should_be_able_to_resolve_common_dependencies()
    {
        _serviceProvider.CheckServiceRegistration<FireblocksApiConfiguration>();
        _serviceProvider.CheckServiceRegistration<IDateTimeProvider, DateTimeProvider>();
        _serviceProvider.CheckServiceRegistration<IBearerCredentialsProvider, BearerCredentialsProvider>();
        _serviceProvider.CheckServiceRegistration<IFireblocksCredentialsProvider, ApiKeyCredentialsProvider>();
        _serviceProvider.CheckServiceRegistration<IClientConfigurator, ClientConfigurator>();
        _serviceProvider.CheckServiceRegistration<IFireblocksApiClientsFactory, FireblocksApiClientsFactory>();
    }
    
    [Fact]
    public void AddApiClients_throws_if_config_is_missing()
    {
        var configuration = JsonConfigurationHelper.BuildFromJson("{ }");
        var serviceCollection = new ServiceCollection();
        var action = () => serviceCollection.AddFireblocksClient(configuration);

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void Expected_http_clients_are_registered()
    {
        var apiClientInterfaces = typeof(FireblocksApiConfiguration)
            .Assembly
            .GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("Client"))
            .ToList();

        apiClientInterfaces.Should().HaveCountGreaterThan(0);

        foreach (var apiClientInterface in apiClientInterfaces)
            _serviceProvider.CheckServiceRegistration(apiClientInterface);
    }
}