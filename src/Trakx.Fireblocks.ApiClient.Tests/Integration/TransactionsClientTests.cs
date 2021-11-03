using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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
            var response = await _fireblocksClient.GetTransactionsAsync();
            response.Result.Count.Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task CreateTransactionAsync_should_create_a_new_transaction()
        {
            var transaction = _mockCreator.GetRandomTransaction();
            var createResponse = await _fireblocksClient.CreateTransactionAsync(transaction);
            var id = createResponse.Result.Id;
            var getResponse = await _fireblocksClient.GetTransactionAsync(id, CancellationToken.None);
            var actualTrans = getResponse.Result;
            actualTrans.AssetId.Should().Be(transaction.AssetId);
            actualTrans.Amount.Should().Be(transaction.Amount);
            actualTrans.CustomerRefId.Should().Be(transaction.CustomerRefId);
        }
    }
}