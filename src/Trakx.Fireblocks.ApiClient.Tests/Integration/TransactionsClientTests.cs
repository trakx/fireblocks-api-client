using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Utils.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class TransactionsClientTests : FireblocksClientTestsBase
    {
        private readonly ITransactionsClient _fireblocksClient;
        private readonly MockCreator _mockCreator;

        public TransactionsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
            : base(apiFixture, output)
        {
            _mockCreator = new MockCreator(output);
            _fireblocksClient = ServiceProvider.GetRequiredService<ITransactionsClient>();
        }

        [Fact]
        public async Task GetTransactionsAsync_should_query_ethereum_transactions_when_passing_eth_asset_id()
        {
            var assetId = "ETH";
            var response = await _fireblocksClient.GetTransactionsAsync(assets: assetId, limit: 2);
            response.Result.Should().NotBeEmpty();
            response.Result.ForEach(f => f.AssetId.Should().Be(assetId));
        }

        [Fact]
        public async Task TransactionsAllAsync_should_query_ethereum_transactions_when_passing_eth_asset_id()
        {
            var transaction = _mockCreator.GetRandomTransaction();
            var response = await _fireblocksClient.CreateTransactionAsync(transaction);
            response.Result.Status.Should().Be("ok");
        }
    }
}