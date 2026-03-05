using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Trakx.Fireblocks.ApiClient.Tests.Integration.Base;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class GasStationClientTests : FireblocksClientTestsBase
{
    private readonly IGas_stationsClient _gasStationClient;

    public GasStationClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _gasStationClient = _serviceProvider.GetRequiredService<IGas_stationsClient>();
    }

    [Fact]
    public async Task GetGasStationInfo_should_return_current_configuration()
    {
        var response = await _gasStationClient.GetGasStationInfoAsync();
        response.Content.Should().NotBeNull();
        response.Content.Configuration.Should().NotBeNull();

        var raw = JsonConvert.SerializeObject(response.Content, Formatting.Indented);
        _logger.Information("Global Gas Station response:\n{Raw}", raw);
    }

    /// <summary>
    /// Queries per-asset gas station config for every asset that has a balance.
    /// Use this to audit the current state before reconfiguring.
    /// </summary>
    [Theory]
    [InlineData("ETH")]
    [InlineData("ETH-AETH")]        // Arbitrum
    [InlineData("BASECHAIN_ETH")]   // Base
    [InlineData("MATIC_POLYGON")]   // Polygon
    [InlineData("AVAX")]            // Avalanche C-Chain
    [InlineData("ETH_ZKSYNC_ERA")]  // zkSync Era
    [InlineData("ETH_ZKEVM")]       // Polygon zkEVM
    [InlineData("SONIC")]           // Sonic
    [InlineData("MONAD")]           // Monad
    public async Task GetGasStationByAssetId_should_return_configuration_for_asset(string assetId)
    {
        var response = await _gasStationClient.GetGasStationByAssetIdAsync(assetId);
        response.Content.Should().NotBeNull();
        response.Content.Configuration.Should().NotBeNull();

        var raw = JsonConvert.SerializeObject(response.Content, Formatting.Indented);
        _logger.Information("Gas Station config for {AssetId}:\n{Raw}", assetId, raw);
    }

    /// <summary>
    /// Reconfigures gas station per asset with values appropriate for each chain.
    ///
    /// gasThreshold = minimum balance before auto-refuel triggers
    /// gasCap       = balance to top-up to when refueling
    /// maxGasPrice  = max gas price in gwei (null = unlimited)
    ///
    /// Values are in the native gas token of each chain.
    /// Rough reasoning (March 2026):
    ///   - ETH mainnet:  ERC-20 transfer ~65k gas x ~30 gwei = 0.002 ETH.
    ///                   Threshold 0.005, cap 0.01 gives ~5-15 txs per wallet.
    ///   - L2s (Arb/Base/zkSync/zkEVM): gas is 10-100x cheaper than mainnet.
    ///                   Threshold 0.002, cap 0.005 gives dozens of txs.
    ///   - Polygon:      POL is ~$0.30. A tx costs ~0.005 POL.
    ///                   Threshold 1, cap 3 gives hundreds of txs.
    ///   - AVAX:         ~25 nAVAX gas price, tx = 0.001 AVAX.
    ///                   Threshold 0.1, cap 0.3 gives many txs.
    ///   - Sonic:        S is ~$0.50. Gas is cheap.
    ///                   Threshold 1, cap 3 gives many txs.
    ///   - Monad:        MON is cheap, gas is very cheap.
    ///                   Threshold 0.01, cap 0.03.
    ///   - SOL:          Not supported by Fireblocks gas station.
    /// </summary>
    [Theory(Skip = "This is not a test but a configuration tool, which actually changes the workspace configuration.")]
    [InlineData(             "ETH",             "0.005",    "0.01",  null)]
    [InlineData(             "ETH-AETH",        "0.002",    "0.005", null)]   // Arbitrum
    [InlineData(             "BASECHAIN_ETH",   "0.002",    "0.005", null)]   // Base
    [InlineData(             "MATIC_POLYGON",   "1",        "3",     null)]   // Polygon
    [InlineData(             "AVAX",            "0.1",      "0.3",   null)]   // Avalanche
    [InlineData(             "ETH_ZKSYNC_ERA",  "0.002",    "0.005", null)]   // zkSync Era
    [InlineData(             "ETH_ZKEVM",       "0.002",    "0.005", null)]   // Polygon zkEVM
    [InlineData(             "SONIC",           "1",        "3",     null)]   // Sonic
    [InlineData(             "MONAD",           "0.01",     "0.03",  null)]   // Monad
    public async Task UpdateGasStationConfiguration_per_asset(
        string assetId, string gasThreshold, string gasCap, string? maxGasPrice)
    {
        var config = new GasStationConfiguration
        {
            GasThreshold = gasThreshold,
            GasCap = gasCap,
            MaxGasPrice = maxGasPrice,
        };

        _logger.Information("Updating Gas Station for {AssetId}: threshold={Threshold}, cap={Cap}, maxGasPrice={MaxGasPrice}",
            assetId, gasThreshold, gasCap, maxGasPrice ?? "unlimited");

        await _gasStationClient.UpdateGasStationConfigurationByAssetIdAsync(assetId, config);

        // read back to confirm the values were applied
        var response = await _gasStationClient.GetGasStationByAssetIdAsync(assetId);
        response.Content.Should().NotBeNull();
        response.Content.Configuration.Should().NotBeNull();
        response.Content.Configuration.GasThreshold.Should().Be(gasThreshold);
        response.Content.Configuration.GasCap.Should().Be(gasCap);
        response.Content.Configuration.MaxGasPrice.Should().Be(maxGasPrice);

        var raw = JsonConvert.SerializeObject(response.Content, Formatting.Indented);
        _logger.Information("Gas Station config for {AssetId} after update:\n{Raw}", assetId, raw);
    }
}