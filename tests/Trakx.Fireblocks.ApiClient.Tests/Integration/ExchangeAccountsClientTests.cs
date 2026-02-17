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
        response.Content.Exchanges.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetExchangeAccountsCredentialsPublicKeyAsync_should_return_public_key()
    {
        var response = await _exchangeAccountsClient.GetExchangeAccountsCredentialsPublicKeyAsync();
        response.Content.Should().NotBeNull();
        response.Content.PublicKey.Should().NotBeNullOrEmpty();
        response.Content.PublicKey.Should().StartWith("-----BEGIN PUBLIC KEY-----");
    }

    [Fact]
    public async Task List_exchange_accounts_with_details()
    {
        string? after = null;

        do
        {
            var response = await _exchangeAccountsClient.GetPagedExchangeAccountsAsync(5, after: after);
            var page = response.Content;

            foreach (var exchange in page.Exchanges)
            {
                _logger.Information("Exchange: Id={Id}, Name={Name}, Type={Type}, Status={Status}, IsSubaccount={IsSubaccount}",
                    exchange.Id, exchange.Name, exchange.Type, exchange.Status, exchange.IsSubaccount);

                if (exchange.Assets is { Count: > 0 })
                {
                    foreach (var asset in exchange.Assets)
                    {
                        _logger.Information("  Asset: Id={AssetId}, Balance={Balance}, Available={Available}, Total={Total}",
                            asset.Id, asset.Balance, asset.Available, asset.Total);
                    }
                }

                if (exchange.TradingAccounts is not { Count: > 0 }) continue;
                foreach (var tradingAccount in exchange.TradingAccounts)
                {
                    _logger.Information("  TradingAccount: Name={Name}, Type={Type}", tradingAccount.Name, tradingAccount.Type);
                }
            }

            after = page.Paging?.After;
        } while (!string.IsNullOrEmpty(after));
    }
}
