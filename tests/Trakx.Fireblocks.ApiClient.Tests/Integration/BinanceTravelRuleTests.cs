using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Fireblocks.ApiClient.TravelRule;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public partial class TransactionsClientTests
{
    [Fact(Skip = "I hope we will never have to test this again 🙈")]
    public async Task WithdrawFromBinance_should_transfer_JUP_to_vault()
    {
        var binanceTravelRuleService = _serviceProvider.GetRequiredService<IBinanceTravelRuleService>();
        var transactionsClient = _serviceProvider.GetRequiredService<ITransactionsClient>();

        var extraParameters = await binanceTravelRuleService.BuildWithdrawalExtraParametersAsync();

        var request = new TransactionRequest
        {
            Operation = TransactionOperation.TRANSFER,
            AssetId = "JUP_SOL",
            Amount = "10",
            Note = "test travel rule compliance",
            CustomerRefId = Guid.NewGuid().ToString(),
            Source = new TransferPeerPath
            {
                Type = TransferPeerPathType.EXCHANGE_ACCOUNT,
                Id = "7b29fd51-6098-4c45-8471-4195cdcbdd70",
            },
            Destination = new DestinationTransferPeerPath
            {
                Type = TransferPeerPathType.VAULT_ACCOUNT,
                Id = "0",
            },
            ExtraParameters = extraParameters,
        };

        var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented);
        _logger.Information("TransactionRequest payload:\n{Payload}", serialized);

        var response = await transactionsClient.CreateTransactionAsync(body: request);

        response.Content.Should().NotBeNull();
        response.Content.Id.Should().NotBeNullOrEmpty();

        _logger.Information("Withdrawal transaction created: Id={Id}, Status={Status}",
            response.Content.Id, response.Content.Status);
    }

    [Fact(Skip = "I hope we will never have to test this again 🙈")]
    public async Task DepositToBinance_should_transfer_JUP_from_vault()
    {
        var binanceTravelRuleService = _serviceProvider.GetRequiredService<IBinanceTravelRuleService>();
        var transactionsClient = _serviceProvider.GetRequiredService<ITransactionsClient>();

        var extraParameters = await binanceTravelRuleService.BuildDepositExtraParametersAsync();

        var request = new TransactionRequest
        {
            Operation = TransactionOperation.TRANSFER,
            AssetId = "JUP_SOL",
            Amount = "10",
            Note = "test travel rule compliance",
            CustomerRefId = Guid.NewGuid().ToString(),
            Source = new TransferPeerPath
            {
                Type = TransferPeerPathType.VAULT_ACCOUNT,
                Id = "0",
            },
            Destination = new DestinationTransferPeerPath
            {
                Type = TransferPeerPathType.EXCHANGE_ACCOUNT,
                Id = "7b29fd51-6098-4c45-8471-4195cdcbdd70",
            },
            ExtraParameters = extraParameters,
        };

        var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented);
        _logger.Information("TransactionRequest payload:\n{Payload}", serialized);

        var response = await transactionsClient.CreateTransactionAsync(body: request);

        response.Content.Should().NotBeNull();
        response.Content.Id.Should().NotBeNullOrEmpty();

        _logger.Information("Deposit transaction created: Id={Id}, Status={Status}",
            response.Content.Id, response.Content.Status);
    }
}