using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class ExchangeAccountsClientTests : FireblocksClientTestsBase
{
    private readonly IExchange_accountsClient _exchangeAccountsClient;

    public ExchangeAccountsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _exchangeAccountsClient = ServiceProvider.GetRequiredService<IExchange_accountsClient>();
    }

    [Fact]
    public async Task Exchange_accountsAllAsync_should_return_all_accounts()
    {
        var response = await _exchangeAccountsClient.Exchange_accountsAllAsync();
        response.Content.Should().BeEmpty();
    }
}