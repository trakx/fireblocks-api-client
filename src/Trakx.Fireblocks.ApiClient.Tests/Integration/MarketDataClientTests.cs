using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class MarketDataClientTests : FireblocksClientTestsBase
    {
        private readonly IMarketDataClient _marketDataClient;

        public MarketDataClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture, output)
        {
            _marketDataClient = ServiceProvider.GetRequiredService<IMarketDataClient>();
        }

        [Fact]
        public async Task GetTicker_should_return_tickers_for_all_exchanges()
        {
            var tasks = Enum.GetValues(typeof(Exchange)).Cast<Exchange>().Select(async exchange =>
            {
                try
                {
                    var tickers = await _marketDataClient.GetTickerAsync(exchange);
                    tickers.Result.Count.Should().BeGreaterThan(10);
                    var knownSymbols = tickers.Result.Select(t => t.Symbol);
                    Logger.Information("Exchange {exchange} has tickers:" +
                                       Environment.NewLine + "{tickers}", exchange,
                        string.Join(",", knownSymbols));
                }
                catch (Exception exception)
                {
                    Logger.Warning(exception, "Failed to get tickers for exchange {exchange}", exchange);
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task GetTicker_should_return_all_tickers_including_OKB_from_okex()
        {
            var tickers = await _marketDataClient.GetTickerAsync(Exchange.Okex);
            tickers.Result.Count.Should().BeGreaterThan(10);
            var knownSymbols = tickers.Result.Select(t => t.Symbol).ToList();
            knownSymbols.Should().Contain("OKB");
            Logger.Information(string.Join(",", knownSymbols));
        }

    }
}
