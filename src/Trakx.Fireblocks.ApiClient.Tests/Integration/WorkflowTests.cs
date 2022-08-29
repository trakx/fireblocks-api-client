using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Utils.Extensions;
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

    [Fact(Skip = "in progress")]
    public async Task Sending_across_vault_accounts_should_work()
    {
        var vaults = await _vaultClient.AccountsAllAsync();
        Logger.Information("Retrieved vault accounts: {accounts}",
            JsonSerializer.Serialize(vaults.Result, new JsonSerializerOptions{WriteIndented = true}));

        var defaultVaultIndex =
            vaults.Result.First(v => v.Name.Equals("default", StringComparison.InvariantCultureIgnoreCase)).Id;
        var testIsinVaultIndex =
            vaults.Result.First(v => v.Name.Equals("TestIsin1", StringComparison.InvariantCultureIgnoreCase)).Id;

        var transactionRequest = new TransactionRequest
        {
            FailOnLowFee = true,
            Amount = 0.01d,
            AssetId = "USDc",
            Destination = new DestinationTransferPeerPath
            {
                Id = testIsinVaultIndex,
                Type = TransferPeerPathType.VAULT_ACCOUNT,
            },
            Source = new TransferPeerPath()
            {
                Id = defaultVaultIndex,
                Type = TransferPeerPathType.VAULT_ACCOUNT
            },
            Note = "testing production signer api key",
            Operation = TransactionOperation.TRANSFER,
            NetworkFee = null,
            GasPrice = null,
            GasLimit = 0,


            //FeeLevel = TransactionRequestFeeLevel.MEDIUM
        };

        var sendTransaction = await _transactionsClient.TransactionsPOSTAsync(transactionRequest);

        var transactionResponse = sendTransaction.Result;
        transactionResponse.Id.Should().NotBeNullOrEmpty();
        //transactionResponse.Status.Should().Be();

        var retrievedTransaction = await _transactionsClient.TransactionsGETAsync(transactionResponse.Id);
        retrievedTransaction.Result.Amount.Should().Be(0.01d);
    }
}
