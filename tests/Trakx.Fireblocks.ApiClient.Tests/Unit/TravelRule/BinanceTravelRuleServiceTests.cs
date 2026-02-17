using Trakx.Common.ApiClient;
using Trakx.Fireblocks.ApiClient.TravelRule;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit.TravelRule;

public class BinanceTravelRuleServiceTests
{
    private readonly BinanceTravelRuleService _sut;
    private readonly ITransactionsClient _transactionsClient;

    public BinanceTravelRuleServiceTests()
    {
        _transactionsClient = Substitute.For<ITransactionsClient>();
        _transactionsClient.CreateTransactionAsync(
                Arg.Any<Guid?>(), Arg.Any<string>(), Arg.Any<TransactionRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => new Response<CreateTransactionResponse>(
                200,
                new Dictionary<string, IEnumerable<string>>(),
                new CreateTransactionResponse { Id = "tx-123", Status = "SUBMITTED" }));

        _sut = new BinanceTravelRuleService(_transactionsClient);
    }

    [Fact]
    public async Task BuildWithdrawalExtraParametersAsync_should_return_valid_pii_data_structure()
    {
        var result = await _sut.BuildWithdrawalExtraParametersAsync();

        result.Should().NotBeNull();
        result.AdditionalProperties.Should().ContainKey("piiData");

        var piiData = result.AdditionalProperties["piiData"] as Dictionary<string, object>;
        piiData.Should().NotBeNull();
        piiData!["type"].Should().Be("exchange-service-travel-rule");
        piiData["typeVersion"].Should().Be("1.0.0");

        var data = piiData["data"] as Dictionary<string, object>;
        data.Should().NotBeNull();

        var beneficiary = data!["beneficiary"] as Dictionary<string, object>;
        beneficiary.Should().NotBeNull();
        beneficiary!["participantRelationshipType"].Should().Be("FirstParty");
        beneficiary["entityType"].Should().Be("Business");

        var transactionData = data["transactionData"] as Dictionary<string, object>;
        var withdraw = (transactionData!["withdraw"] as Dictionary<string, object>)!;
        withdraw["isAddressVerified"].Should().Be(true);

        data.Should().NotContainKey("beneficiaryVASP");

        var originatingVasp = data["originatingVASP"] as Dictionary<string, object>;
        originatingVasp!["vaspCountry"].Should().Be("FR");
    }

    [Fact]
    public async Task BuildDepositExtraParametersAsync_should_return_valid_pii_data_structure()
    {
        var result = await _sut.BuildDepositExtraParametersAsync();

        result.Should().NotBeNull();
        result.AdditionalProperties.Should().ContainKey("piiData");

        var piiData = result.AdditionalProperties["piiData"] as Dictionary<string, object>;
        piiData.Should().NotBeNull();
        piiData!["type"].Should().Be("exchange-service-travel-rule");
        piiData["typeVersion"].Should().Be("1.0.0");

        var data = piiData["data"] as Dictionary<string, object>;
        data.Should().NotBeNull();

        var originator = data!["originator"] as Dictionary<string, object>;
        originator.Should().NotBeNull();
        originator!["participantRelationshipType"].Should().Be("FirstParty");
        originator["entityType"].Should().Be("Business");

        var originatingVasp = data["originatingVASP"] as Dictionary<string, object>;
        originatingVasp!["vaspName"].Should().Be("Binance");

        var transactionData = data["transactionData"] as Dictionary<string, object>;
        var deposit = (transactionData!["deposit"] as Dictionary<string, object>)!;
        deposit["isAddressVerified"].Should().Be(true);

        data.Should().NotContainKey("beneficiaryVASP");
    }

    [Fact]
    public async Task WithdrawFromBinanceAsync_should_create_transaction_with_correct_peer_paths()
    {
        var result = await _sut.WithdrawFromBinanceAsync("exchange-1", "vault-2", "BTC", "0.5", "test note");

        result.Id.Should().Be("tx-123");

        await _transactionsClient.Received(1).CreateTransactionAsync(
            Arg.Any<Guid?>(),
            Arg.Any<string>(),
            Arg.Is<TransactionRequest>(r =>
                r.Source.Type == TransferPeerPathType.EXCHANGE_ACCOUNT &&
                r.Source.Id == "exchange-1" &&
                r.Destination.Type == TransferPeerPathType.VAULT_ACCOUNT &&
                r.Destination.Id == "vault-2" &&
                r.AssetId == "BTC" &&
                r.Amount == "0.5" &&
                r.Note == "test note" &&
                r.ExtraParameters != null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DepositToBinanceAsync_should_create_transaction_with_correct_peer_paths()
    {
        var result = await _sut.DepositToBinanceAsync("exchange-1", "vault-2", "ETH", "1.0");

        result.Id.Should().Be("tx-123");

        await _transactionsClient.Received(1).CreateTransactionAsync(
            Arg.Any<Guid?>(),
            Arg.Any<string>(),
            Arg.Is<TransactionRequest>(r =>
                r.Source.Type == TransferPeerPathType.VAULT_ACCOUNT &&
                r.Source.Id == "vault-2" &&
                r.Destination.Type == TransferPeerPathType.EXCHANGE_ACCOUNT &&
                r.Destination.Id == "exchange-1" &&
                r.AssetId == "ETH" &&
                r.Amount == "1.0" &&
                r.ExtraParameters != null),
            Arg.Any<CancellationToken>());
    }

    [Fact(Skip = "Integration test — requires real Fireblocks credentials and Binance exchange account")]
    public async Task WithdrawFromBinance_should_transfer_JUP_to_vault()
    {
        await _sut.WithdrawFromBinanceAsync(
            exchangeAccountId: "7b29fd51-6098-4c45-8471-4195cdcbdd70",
            vaultAccountId: "0",
            assetId: "JUP_SOL",
            amount: "10",
            note: "test travel rule compliance",
            customerRefId: Guid.NewGuid().ToString());
    }
}
