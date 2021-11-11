using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class TransactionsClientTests : FireblocksClientTestsBase
    {
        private readonly ITransactionsClient _transactionsClient;
        private readonly MockCreator _mockCreator;

        public TransactionsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
            : base(apiFixture, output)
        {
            _mockCreator = new MockCreator(output);
            _transactionsClient = ServiceProvider.GetRequiredService<ITransactionsClient>();
        }

        [Fact]
        public async Task GetTransactionsAsync_should_query_ethereum_transactions_when_passing_eth_asset_id()
        {
            var response = await _transactionsClient.GetTransactionsAsync(assets: "ETH_TEST", limit: 2);
            response.Result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateTransactionAsync_should_create_a_new_transaction()
        {
            var trans = new TransactionRequest
            {
                Amount = 0.1,
                Destination = new DestinationTransferPeerPath
                {
                    Id = "0",
                    Type = TransferPeerPathType.VAULT_ACCOUNT,
                },
                Fee = 0.1,
                Note = "nonono",
                Operation = TransactionOperation.SUPPLY_TO_COMPOUND,
                Source = new TransferPeerPath
                {
                    Id = "0",
                    Type = TransferPeerPathType.VAULT_ACCOUNT,
                },
                AutoStaking = true,
                AssetId = "ETH_TEST",
                CpuStaking = 0.0,
                FeeLevel = TransactionRequestFeeLevel.LOW,
                GasLimit = 0.1,
                GasPrice = 0.1,
                MaxFee = "1",
                NetworkFee = 0.1,
                NetworkStaking = 0,
                CustomerRefId = "1",
                FailOnLowFee = true
            };
            var createResponse = await _transactionsClient.CreateTransactionAsync(trans);
            var id = createResponse.Result.Id;

            // After write op, fireblocks needs some time to set row status. :(
            Thread.Sleep(2000);

            var getResponse = await _transactionsClient.GetTransactionAsync(id, CancellationToken.None);
            var actualTrans = getResponse.Result;
            actualTrans.AssetId.Should().Be(trans.AssetId);
            actualTrans.Amount.Should().Be(trans.Amount);
        }

    }
}
