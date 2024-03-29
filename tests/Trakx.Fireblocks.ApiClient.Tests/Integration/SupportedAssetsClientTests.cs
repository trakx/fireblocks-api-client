using Microsoft.Extensions.DependencyInjection;
using Trakx.Fireblocks.ApiClient.Tests.Integration.Base;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class SupportedAssetsClientTests : FireblocksClientTestsBase
{

    private readonly IBlockchains_and_assetsClient _supportedAssetsClient;

    public SupportedAssetsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _supportedAssetsClient = _serviceProvider.GetRequiredService<IBlockchains_and_assetsClient>();
    }

    [Fact]
    public async Task GetSupportedAssetsAsync_should_query_all_assets()
    {
        var assets = await _supportedAssetsClient.GetSupportedAssetsAsync();
        IEnumerable<string> assetNames = assets.Content.Select(a => $"{a.Id} - {a.Name}");
        _logger.Information("Found assets: {assets}", string.Join(",", assetNames));
        var asset = assets.Content.Where(i => i.Id.Contains("ftt", StringComparison.InvariantCultureIgnoreCase)).ToList();
        assets.Content.Should().NotBeEmpty();
    }
}