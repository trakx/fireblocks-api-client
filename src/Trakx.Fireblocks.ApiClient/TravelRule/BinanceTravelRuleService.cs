namespace Trakx.Fireblocks.ApiClient.TravelRule;

/// <inheritdoc />
internal class BinanceTravelRuleService(
    ITransactionsClient transactionsClient) : IBinanceTravelRuleService
{
    /// <inheritdoc />
    public Task<ExtraParameters> BuildWithdrawalExtraParametersAsync(CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            ["beneficiary"] = new Dictionary<string, object>
            {
                ["participantRelationshipType"] = "FirstParty",
                ["entityType"] = "Business"
            },
            ["transactionData"] = new Dictionary<string, object>
            {
                ["withdraw"] = new Dictionary<string, object>
                {
                    ["isAddressVerified"] = true
                }
            },
            ["originatingVASP"] = new Dictionary<string, object>
            {
                ["vaspCountry"] = "FR"
            }
        };

        var piiData = new Dictionary<string, object>
        {
            ["type"] = "exchange-service-travel-rule",
            ["typeVersion"] = "1.0.0",
            ["data"] = data
        };

        var extraParameters = new ExtraParameters();
        extraParameters.AdditionalProperties["piiData"] = piiData;
        return Task.FromResult(extraParameters);
    }

    /// <inheritdoc />
    public Task<ExtraParameters> BuildDepositExtraParametersAsync(CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            ["originator"] = new Dictionary<string, object>
            {
                ["participantRelationshipType"] = "FirstParty",
                ["entityType"] = "Business"
            },
            ["transactionData"] = new Dictionary<string, object>
            {
                ["deposit"] = new Dictionary<string, object>
                {
                    ["isAddressVerified"] = true
                }
            },
            ["originatingVASP"] = new Dictionary<string, object>
            {
                ["vaspName"] = "Binance"
            }
        };

        var piiData = new Dictionary<string, object>
        {
            ["type"] = "exchange-service-travel-rule",
            ["typeVersion"] = "1.0.0",
            ["data"] = data
        };

        var extraParameters = new ExtraParameters();
        extraParameters.AdditionalProperties["piiData"] = piiData;
        return Task.FromResult(extraParameters);
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
}
