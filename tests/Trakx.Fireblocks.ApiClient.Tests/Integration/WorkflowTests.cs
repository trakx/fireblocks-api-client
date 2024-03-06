using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Fireblocks.ApiClient.Tests.Integration.Base;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class WorkflowTests : FireblocksClientTestsBase
{
    private readonly IVaultsClient _vaultClient;
    private readonly ITransactionsClient _transactionsClient;
    private readonly INetwork_connectionsClient _networkClient;

    public WorkflowTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture, output)
    {
        _vaultClient = _serviceProvider.GetRequiredService<IVaultsClient>();
        _networkClient = _serviceProvider.GetRequiredService<INetwork_connectionsClient>();
        _transactionsClient = _serviceProvider.GetRequiredService<ITransactionsClient>();
    }

    [Fact(Skip = "This is actually sending money across the network")]
    //[Fact]
    public async Task Sending_across_network_connections_should_work()
    {
        const string sourceVaultName = "default";

        var trakxVaults = await _vaultClient.AccountsAllAsync();
        _logger.Information("Retrieved vault accounts: {accounts}",
            JsonSerializer.Serialize(trakxVaults.Content, new JsonSerializerOptions { WriteIndented = true }));

        var partnerId = await GetNarrativeNetworkConnectionId();

        var sourceVaultIndex =
            trakxVaults.Content.First(v => v.Name.Equals(sourceVaultName, StringComparison.InvariantCultureIgnoreCase)).Id;

        var transactionRequest = new TransactionRequest
        {
            FailOnLowFee = true,
            Amount = 0.01d,
            AssetId = "USDC",
            Destination = new DestinationTransferPeerPath
            {
                Id = partnerId,
                Type = TransferPeerPathType.NETWORK_CONNECTION,
            },
            Source = new TransferPeerPath
            {
                Id = sourceVaultIndex,
                Type = TransferPeerPathType.VAULT_ACCOUNT
            },
            Note = "testing production signer api key",
            Operation = TransactionOperation.TRANSFER,
            FeeLevel = TransactionRequestFeeLevel.MEDIUM
        };

        var sendTransaction = await _transactionsClient.TransactionsPOSTAsync(transactionRequest);

        var transactionResponse = sendTransaction.Content;
        transactionResponse.Id.Should().NotBeNullOrEmpty();

        var retrievedTransaction = await _transactionsClient.TransactionsGETAsync(transactionResponse.Id);
        retrievedTransaction.Content.Amount.Should().Be(0.01d);
    }

    private async Task<string> GetNarrativeNetworkConnectionId()
    {
        var networkPartners = await _networkClient.Network_connectionsAllAsync();
        networkPartners.Content.Count.Should().BeGreaterThan(0);

        _logger.Information("Retrieved network connections: {connections}",
            JsonSerializer.Serialize(networkPartners.Content, new JsonSerializerOptions { WriteIndented = true }));
        const string partnerFullName = "Narrative PCC Limited - acting in respect of Protected CH1181262820 Cell";

        networkPartners.Content.Should().Contain(r => r.RemoteNetworkId.Name.Equals(partnerFullName));

        var peerId = networkPartners.Content.Single(r => r.RemoteNetworkId.Name.Equals(partnerFullName)).Id;
        return peerId;
    }
}