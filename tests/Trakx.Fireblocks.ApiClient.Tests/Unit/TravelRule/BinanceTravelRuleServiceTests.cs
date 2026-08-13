using System.Security.Cryptography;
using System.Text;
using Trakx.Common.ApiClient;
using Trakx.Fireblocks.ApiClient.TravelRule;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit.TravelRule;

public class BinanceTravelRuleServiceTests : IDisposable
{
    private readonly BinanceTravelRuleService _sut;
    private readonly ITransactionsClient _transactionsClient;
    private readonly RSA _rsa;

    public BinanceTravelRuleServiceTests()
    {
        _rsa = RSA.Create(2048);
        var publicKeyPem = _rsa.ExportSubjectPublicKeyInfoPem();

        var exchangeAccountsClient = Substitute.For<IExchange_accountsClient>();
        exchangeAccountsClient.GetExchangeAccountsCredentialsPublicKeyAsync(Arg.Any<CancellationToken>())
            .Returns(new Response<ExchangeCredentialsPublicKeyResponse>(
                200,
                new Dictionary<string, IEnumerable<string>>(),
                new ExchangeCredentialsPublicKeyResponse { PublicKey = publicKeyPem }));

        _transactionsClient = Substitute.For<ITransactionsClient>();
        _transactionsClient.CreateTransactionAsync(
                Arg.Any<Guid?>(), Arg.Any<string>(), Arg.Any<TransactionRequest>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => new Response<CreateTransactionResponse>(
                200,
                new Dictionary<string, IEnumerable<string>>(),
                new CreateTransactionResponse { Id = "tx-123", Status = "SUBMITTED" }));

        var encryptionService = new RsaEncryptionService();
        _sut = new BinanceTravelRuleService(exchangeAccountsClient, _transactionsClient, encryptionService);
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
        DecryptValue(beneficiary!["participantRelationshipType"]).Should().Be("FirstParty");
        DecryptValue(beneficiary["entityType"]).Should().Be("Business");

        var transactionData = data["transactionData"] as Dictionary<string, object>;
        var withdraw = (transactionData!["withdraw"] as Dictionary<string, object>)!;
        DecryptBool(withdraw["isAddressVerified"]).Should().BeTrue();

        //explicitly diverge from Fireblocks documentation, the doc is wrong.
        data.Should().NotContainKey("beneficiaryVASP");

        var originatingVasp = data["originatingVASP"] as Dictionary<string, object>;
        DecryptValue(originatingVasp!["vaspCountry"]).Should().Be("FR");
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
        DecryptValue(originator!["participantRelationshipType"]).Should().Be("FirstParty");
        DecryptValue(originator["entityType"]).Should().Be("Business");

        var originatingVasp = data["originatingVASP"] as Dictionary<string, object>;
        DecryptValue(originatingVasp!["vaspName"]).Should().Be("Binance");

        var transactionData = data["transactionData"] as Dictionary<string, object>;
        var deposit = (transactionData!["deposit"] as Dictionary<string, object>)!;
        DecryptBool(deposit["isAddressVerified"]).Should().BeTrue();

        //explicitly diverge from Fireblocks documentation, the doc is wrong.
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
                r!.Source.Type == TransferPeerPathType.EXCHANGE_ACCOUNT &&
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
                r!.Source.Type == TransferPeerPathType.VAULT_ACCOUNT &&
                r.Source.Id == "vault-2" &&
                r.Destination.Type == TransferPeerPathType.EXCHANGE_ACCOUNT &&
                r.Destination.Id == "exchange-1" &&
                r.AssetId == "ETH" &&
                r.Amount == "1.0" &&
                r.ExtraParameters != null),
            Arg.Any<CancellationToken>());
    }

    private string DecryptValue(object encryptedBase64)
    {
        var encryptedBytes = Convert.FromBase64String((string)encryptedBase64);
        var decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private bool DecryptBool(object encryptedBase64)
    {
        var encryptedBytes = Convert.FromBase64String((string)encryptedBase64);
        var decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        return BitConverter.ToBoolean(decryptedBytes);
    }

    public void Dispose()
    {
        _rsa.Dispose();
    }
}