namespace Trakx.Fireblocks.ApiClient.TravelRule;

/// <inheritdoc />
internal class BinanceTravelRuleService(
    IExchange_accountsClient exchangeAccountsClient,
    ITransactionsClient transactionsClient,
    IRsaEncryptionService encryptionService) : IBinanceTravelRuleService
{
    private string? _cachedPublicKeyPem;

    /// <inheritdoc />
    public async Task<ExtraParameters> BuildWithdrawalExtraParametersAsync(CancellationToken cancellationToken = default)
    {
        return await BuildExtraParametersAsync(
            partyKey: "beneficiary",
            transactionDirection: "withdraw",
            vaspSection: new Dictionary<string, object>
            {
                ["beneficiaryVASP"] = new Dictionary<string, object> { ["vaspCode"] = "BINANCE" },
                ["originatingVASP"] = new Dictionary<string, object> { ["vaspCountry"] = "FR" }
            },
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExtraParameters> BuildDepositExtraParametersAsync(CancellationToken cancellationToken = default)
    {
        return await BuildExtraParametersAsync(
            partyKey: "originator",
            transactionDirection: "deposit",
            vaspSection: new Dictionary<string, object>
            {
                ["originatingVASP"] = new Dictionary<string, object> { ["vaspName"] = "Binance" },
                ["beneficiaryVASP"] = new Dictionary<string, object> { ["vaspCountry"] = "FR" }
            },
            cancellationToken);
    }

    private async Task<ExtraParameters> BuildExtraParametersAsync(
        string partyKey,
        string transactionDirection,
        Dictionary<string, object> vaspSection,
        CancellationToken cancellationToken)
    {
        var pem = await GetPublicKeyAsync(cancellationToken);

        var data = new Dictionary<string, object>
        {
            [partyKey] = new Dictionary<string, object>
            {
                ["participantRelationshipType"] = encryptionService.Encrypt("FirstParty", pem),
                ["entityType"] = encryptionService.Encrypt("Business", pem)
            },
            ["transactionData"] = new Dictionary<string, object>
            {
                [transactionDirection] = new Dictionary<string, object>
                {
                    ["isAddressVerified"] = encryptionService.Encrypt(true, pem)
                }
            }
        };

        foreach (var (key, value) in vaspSection)
        {
            var plainValues = (Dictionary<string, object>)value;
            var encrypted = new Dictionary<string, object>();
            foreach (var (fieldKey, fieldValue) in plainValues)
                encrypted[fieldKey] = encryptionService.Encrypt((string)fieldValue, pem);
            data[key] = encrypted;
        }

        var piiData = new Dictionary<string, object>
        {
            ["type"] = "exchange-service-travel-rule",
            ["typeVersion"] = "1.0.0",
            ["data"] = data
        };

        var extraParameters = new ExtraParameters();
        extraParameters.AdditionalProperties["piiData"] = piiData;
        return extraParameters;
    }

    /// <inheritdoc />
    public async Task<CreateTransactionResponse> WithdrawFromBinanceAsync(
        string exchangeAccountId, string vaultAccountId, string assetId, string amount,
        string? note = null, string? customerRefId = null, CancellationToken cancellationToken = default)
    {
        var extraParameters = await BuildWithdrawalExtraParametersAsync(cancellationToken);

        var request = new TransactionRequest
        {
            Operation = TransactionOperation.TRANSFER,
            AssetId = assetId,
            Amount = amount,
            Note = note,
            CustomerRefId = customerRefId,
            Source = new TransferPeerPath
            {
                Type = TransferPeerPathType.EXCHANGE_ACCOUNT,
                Id = exchangeAccountId,
            },
            Destination = new DestinationTransferPeerPath
            {
                Type = TransferPeerPathType.VAULT_ACCOUNT,
                Id = vaultAccountId,
            },
            ExtraParameters = extraParameters,
        };

        var response = await transactionsClient.CreateTransactionAsync(body: request, cancellationToken: cancellationToken);
        return response.Content;
    }

    /// <inheritdoc />
    public async Task<CreateTransactionResponse> DepositToBinanceAsync(
        string exchangeAccountId, string vaultAccountId, string assetId, string amount,
        string? note = null, string? customerRefId = null, CancellationToken cancellationToken = default)
    {
        var extraParameters = await BuildDepositExtraParametersAsync(cancellationToken);

        var request = new TransactionRequest
        {
            Operation = TransactionOperation.TRANSFER,
            AssetId = assetId,
            Amount = amount,
            Note = note,
            CustomerRefId = customerRefId,
            Source = new TransferPeerPath
            {
                Type = TransferPeerPathType.VAULT_ACCOUNT,
                Id = vaultAccountId,
            },
            Destination = new DestinationTransferPeerPath
            {
                Type = TransferPeerPathType.EXCHANGE_ACCOUNT,
                Id = exchangeAccountId,
            },
            ExtraParameters = extraParameters,
        };

        var response = await transactionsClient.CreateTransactionAsync(body: request, cancellationToken: cancellationToken);
        return response.Content;
    }

    private async Task<string> GetPublicKeyAsync(CancellationToken cancellationToken)
    {
        return _cachedPublicKeyPem ??= (await exchangeAccountsClient
            .GetExchangeAccountsCredentialsPublicKeyAsync(cancellationToken))
            .Content.PublicKey;
    }
}
