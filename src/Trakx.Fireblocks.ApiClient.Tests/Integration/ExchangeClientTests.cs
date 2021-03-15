using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class ExchangeClientTests : FireblocksClientTestsBase
    {
        private readonly IExchangeAccountsClient _client;

        public ExchangeClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture, output)
        {
            _client = ServiceProvider.GetRequiredService<IExchangeAccountsClient>();
        }

        [Fact]
        public async Task AccountsAllAsync_should_return_all_accounts()
        {
            var response = await _client.Exchange_accountsAllAsync();
            response.Result.Should().BeNullOrEmpty();
        }
    }
}