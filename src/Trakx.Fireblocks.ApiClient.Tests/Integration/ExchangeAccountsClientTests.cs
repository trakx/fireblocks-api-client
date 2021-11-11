using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class ExchangeClientTests : FireblocksClientTestsBase
    {
        private readonly IExchangeAccountsClient _exchangeAccountsClient;

        public ExchangeClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
            : base(apiFixture, output)
        {
            _exchangeAccountsClient = ServiceProvider.GetRequiredService<IExchangeAccountsClient>();
        }

        [Fact]
        public async Task Exchange_accountsAllAsync_should_return_all_accounts()
        {
            var response = await _exchangeAccountsClient.Exchange_accountsAllAsync();
            response.Result.Should().BeEmpty();
        }
    }
}