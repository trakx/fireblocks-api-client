using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Fireblocks.ApiClient.Tests.Integration.Base;
using Trakx.Fireblocks.ApiClient.TravelRule;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class ExchangeAccountsClientTests : FireblocksClientTestsBase
{
    private readonly IExchange_accountsClient _exchangeAccountsClient;

    public ExchangeAccountsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _exchangeAccountsClient = _serviceProvider.GetRequiredService<IExchange_accountsClient>();
    }

    [Fact]
    public async Task Exchange_accountsAllAsync_should_return_all_accounts()
    {
        var response = await _exchangeAccountsClient.GetPagedExchangeAccountsAsync(5);
        response.Content.Exchanges.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetExchangeAccountsCredentialsPublicKeyAsync_should_return_public_key()
    {
        var response = await _exchangeAccountsClient.GetExchangeAccountsCredentialsPublicKeyAsync();
        response.Content.Should().NotBeNull();
        response.Content.PublicKey.Should().NotBeNullOrEmpty();
        response.Content.PublicKey.Should().StartWith("-----BEGIN PUBLIC KEY-----");
    }

    [Fact]
    public async Task List_exchange_accounts_with_details()
    {
        string? after = null;

        do
        {
            var response = await _exchangeAccountsClient.GetPagedExchangeAccountsAsync(5, after: after);
            var page = response.Content;

            foreach (var exchange in page.Exchanges)
            {
                _logger.Information("Exchange: Id={Id}, Name={Name}, Type={Type}, Status={Status}, IsSubaccount={IsSubaccount}",
                    exchange.Id, exchange.Name, exchange.Type, exchange.Status, exchange.IsSubaccount);

                if (exchange.Assets is { Count: > 0 })
                {
                    foreach (var asset in exchange.Assets)
                    {
                        _logger.Information("  Asset: Id={AssetId}, Balance={Balance}, Available={Available}, Total={Total}",
                            asset.Id, asset.Balance, asset.Available, asset.Total);
                    }
                }

                if (exchange.TradingAccounts is not { Count: > 0 }) continue;
                foreach (var tradingAccount in exchange.TradingAccounts)
                {
                    _logger.Information("  TradingAccount: Name={Name}, Type={Type}", tradingAccount.Name, tradingAccount.Type);
                }
            }

            after = page.Paging?.After;
        } while (!string.IsNullOrEmpty(after));
    }

    [Fact]
    public async Task BuildWithdrawalExtraParameters_should_produce_valid_encrypted_pii_data()
    {
        var binanceTravelRuleService = _serviceProvider.GetRequiredService<IBinanceTravelRuleService>();

        var result = await binanceTravelRuleService.BuildWithdrawalExtraParametersAsync();

        result.Should().NotBeNull();
        result.AdditionalProperties.Should().ContainKey("piiData");

        var piiData = result.AdditionalProperties["piiData"] as Dictionary<string, object>;
        piiData.Should().NotBeNull();
        piiData!["type"].Should().Be("exchange-service-travel-rule");
        piiData["typeVersion"].Should().Be("1.0.0");

        var data = piiData["data"] as Dictionary<string, object>;
        data.Should().NotBeNull();
        data.Should().ContainKey("beneficiary");
        data.Should().ContainKey("transactionData");
        data.Should().ContainKey("beneficiaryVASP");
        data.Should().ContainKey("originatingVASP");

        var beneficiary = data!["beneficiary"] as Dictionary<string, object>;
        AssertBase64Encoded(beneficiary!["participantRelationshipType"]);
        AssertBase64Encoded(beneficiary["entityType"]);

        _logger.Information("Withdrawal piiData: {PiiData}",
            JsonSerializer.Serialize(piiData, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact]
    public async Task BuildDepositExtraParameters_should_produce_valid_encrypted_pii_data()
    {
        var binanceTravelRuleService = _serviceProvider.GetRequiredService<IBinanceTravelRuleService>();

        var result = await binanceTravelRuleService.BuildDepositExtraParametersAsync();

        result.Should().NotBeNull();
        result.AdditionalProperties.Should().ContainKey("piiData");

        var piiData = result.AdditionalProperties["piiData"] as Dictionary<string, object>;
        piiData.Should().NotBeNull();
        piiData!["type"].Should().Be("exchange-service-travel-rule");
        piiData["typeVersion"].Should().Be("1.0.0");

        var data = piiData["data"] as Dictionary<string, object>;
        data.Should().NotBeNull();
        data.Should().ContainKey("originator");
        data.Should().ContainKey("originatingVASP");
        data.Should().ContainKey("transactionData");
        data.Should().ContainKey("beneficiaryVASP");

        var originator = data!["originator"] as Dictionary<string, object>;
        AssertBase64Encoded(originator!["participantRelationshipType"]);
        AssertBase64Encoded(originator["entityType"]);

        _logger.Information("Deposit piiData: {PiiData}",
            JsonSerializer.Serialize(piiData, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact]
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

    private static void AssertBase64Encoded(object value)
    {
        var str = value as string;
        str.Should().NotBeNullOrEmpty();
        var action = () => Convert.FromBase64String(str);
        action.Should().NotThrow();
    }
}
