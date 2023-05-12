using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class TransactionsClientTests : FireblocksClientTestsBase
{
    private readonly ITransactionsClient _transactionsClient;

    public TransactionsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _transactionsClient = ServiceProvider.GetRequiredService<ITransactionsClient>();
    }

    [Fact]
    public async Task GetTransactionsAsync_should_query_ethereum_transactions_when_passing_eth_asset_id()
    {
        var response = await _transactionsClient.TransactionsAllAsync(assets: "ETH_TEST3", limit: 2);
        response.Content.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateTransactionAsync_should_create_a_new_transaction()
    {
        var trans = new TransactionRequest
        {
            Amount = 0.001,
            Destination = new DestinationTransferPeerPath
            {
                Id = "0",
                Type = TransferPeerPathType.VAULT_ACCOUNT,
            },
            Note = "nonono",
            Source = new TransferPeerPath
            {
                Id = "2",
                Type = TransferPeerPathType.VAULT_ACCOUNT,
            },
            AssetId = "ETH_TEST3",
            MaxFee = "0.1"
        };
        var createResponse = await _transactionsClient.TransactionsPOSTAsync(trans);
        var id = createResponse.Content.Id;

        var getResponse = await _transactionsClient.TransactionsGETAsync(id, CancellationToken.None);
        var actualTrans = getResponse.Content;
        actualTrans.AssetId.Should().Be(trans.AssetId);
        actualTrans.Amount.Should().Be(trans.Amount);
    }

}
