using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

public sealed class ApiKeyCredentialsProviderTests : IDisposable
{
    private readonly ApiKeyCredentialsProvider _provider;
    private readonly IBearerCredentialsProvider _bearerCredentials;

    public ApiKeyCredentialsProviderTests()
    {
        var fireblocksApiConfiguration = new FireblocksApiConfiguration { ApiPubKey = "pubKey" };
        _bearerCredentials = Substitute.For<IBearerCredentialsProvider>();
        _provider = new ApiKeyCredentialsProvider(fireblocksApiConfiguration, _bearerCredentials);
    }

    [Fact]
    public void AddCredentials_should_call_token_provider_and_add_authorisation_headers()
    {
        var message = new HttpRequestMessage { RequestUri = new Uri("https://test.com/test1/validate") };

        _bearerCredentials.GenerateJwtToken(Arg.Any<HttpRequestMessage>()).ReturnsForAnyArgs("jwt token");
        _provider.AddCredentials(message);

        _bearerCredentials.Received(1).GenerateJwtToken(message);

        message.Headers.GetValues(ApiKeyCredentialsProvider.ApiKeyHeader).Single()
            .Should().Be("pubKey");
        message.Headers.Authorization!.Scheme.Should().Be(ApiKeyCredentialsProvider.JwtScheme);
        message.Headers.Authorization!.Parameter.Should().Be("jwt token");
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}