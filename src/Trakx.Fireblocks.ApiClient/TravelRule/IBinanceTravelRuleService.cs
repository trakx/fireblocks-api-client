namespace Trakx.Fireblocks.ApiClient.TravelRule;

/// <summary>
/// Service for building encrypted PII data required by Binance travel rule compliance.
/// See: https://developers.fireblocks.com/docs/a-developers-guide-to-constructing-encrypted-pii-messages-for-binance-via-fireblocks
/// </summary>
public interface IBinanceTravelRuleService
{
    /// <summary>
    /// Builds the travel rule extra parameters required for Binance withdrawals (exchange to custodian).
    /// </summary>
    Task<ExtraParameters> BuildWithdrawalExtraParametersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds the travel rule extra parameters required for Binance deposits (custodian to exchange).
    /// </summary>
    Task<ExtraParameters> BuildDepositExtraParametersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Withdraws from Binance exchange account to a vault account, with travel rule compliance.
    /// </summary>
    Task<CreateTransactionResponse> WithdrawFromBinanceAsync(
        string exchangeAccountId, string vaultAccountId, string assetId, string amount,
        string? note = null, string? customerRefId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deposits from a vault account to Binance exchange account, with travel rule compliance.
    /// </summary>
    Task<CreateTransactionResponse> DepositToBinanceAsync(
        string exchangeAccountId, string vaultAccountId, string assetId, string amount,
        string? note = null, string? customerRefId = null, CancellationToken cancellationToken = default);
}