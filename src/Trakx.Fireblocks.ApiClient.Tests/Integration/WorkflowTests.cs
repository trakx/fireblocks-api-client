using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class WorkflowTests : FireblocksClientTestsBase
{
    private readonly IVaultsClient _vaultClient;
    private readonly ITransactionsClient _transactionsClient;
    private readonly INetwork_connectionsClient _networkClient;

    public WorkflowTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture, output)
    {
        _vaultClient = ServiceProvider.GetRequiredService<IVaultsClient>();
        _networkClient = ServiceProvider.GetRequiredService<INetwork_connectionsClient>();
        _transactionsClient = ServiceProvider.GetRequiredService<ITransactionsClient>();
    }

    [Fact(Skip = "This is actually sending money across the network")]
    //[Fact]
    public async Task Sending_across_network_connections_should_work()
    {
        const string sourceVaultName = "default";

        var trakxVaults = await _vaultClient.AccountsAllAsync();
        Logger.Information("Retrieved vault accounts: {accounts}",
            JsonSerializer.Serialize(trakxVaults.Result, new JsonSerializerOptions{WriteIndented = true}));

        var partnerId = await GetNarrativeNetworkConnectionId();

        var sourceVaultIndex =
            trakxVaults.Result.First(v => v.Name.Equals(sourceVaultName, StringComparison.InvariantCultureIgnoreCase)).Id;

        var transactionRequest = new TransactionRequest
        {
            FailOnLowFee = true,
            Amount = 0.01d,
            AssetId = "USDC",
            Destination = new DestinationTransferPeerPath
            {
                Id = partnerId,
                Type = TransferPeerPathType. NETWORK_CONNECTION,
            },
            Source = new TransferPeerPath()
            {
                Id = sourceVaultIndex,
                Type = TransferPeerPathType.VAULT_ACCOUNT
            },
            Note = "testing production signer api key",
            Operation = TransactionOperation.TRANSFER,
            NetworkFee = null,
            Fee = null,
            GasPrice = null,
            GasLimit = null,
            FeeLevel = TransactionRequestFeeLevel.MEDIUM
        };

        var sendTransaction = await _transactionsClient.TransactionsPOSTAsync(transactionRequest);

        var transactionResponse = sendTransaction.Result;
        transactionResponse.Id.Should().NotBeNullOrEmpty();

        var retrievedTransaction = await _transactionsClient.TransactionsGETAsync(transactionResponse.Id);
        retrievedTransaction.Result.Amount.Should().Be(0.01d);
    }

    private async Task<string> GetNarrativeNetworkConnectionId()
    {
        var networkPartners = await _networkClient.Network_connectionsAllAsync();
        networkPartners.Result.Count.Should().BeGreaterThan(0);

        Logger.Information("Retrieved network connections: {connections}",
            JsonSerializer.Serialize(networkPartners.Result, new JsonSerializerOptions { WriteIndented = true }));
        const string partnerFullName = "Narrative PCC Limited - acting in respect of Protected CH1181262820 Cell";

        networkPartners.Result.Should().Contain(r => r.RemoteNetworkId.Name.Equals(partnerFullName));

        var peerId = networkPartners.Result.Single(r => r.RemoteNetworkId.Name.Equals(partnerFullName)).Id;
        return peerId;
    }
}
