using Microsoft.Extensions.DependencyInjection;
using Trakx.Fireblocks.ApiClient.Tests.Integration.Base;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class ExchangeAccountsClientTests : FireblocksClientTestsBase
{
    private readonly IExchange_accountsClient _exchangeAccountsClient;

    public ExchangeAccountsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _exchangeAccountsClient = _serviceProvider.GetRequiredService<IExchange_accountsClient>();
    }

    [Fact]
    public async Task Exchange_accountsAllAsync_should_return_all_accounts()
    {
        var response = await _exchangeAccountsClient.GetPagedExchangeAccountsAsync(5);
        response.Content.Exchanges.Should().BeEmpty();
    }
}