using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class SupportedAssetsClientTests : FireblocksClientTestsBase
{

    private readonly ISupported_assetsClient _supportedAssetsClient;

    public SupportedAssetsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _supportedAssetsClient = ServiceProvider.GetRequiredService<ISupported_assetsClient>();
    }

    [Fact]
    public async Task GetSupportedAssetsAsync_should_query_all_assets()
    {
        var assets = await _supportedAssetsClient.Supported_assetsAsync();
        IEnumerable<string> assetNames = assets.Content.Select(a => $"{a.Id} - {a.Name}");
        Logger.Information("Found assets: {assets}", string.Join(",", assetNames));
        var asset = assets.Content.Where(i => i.Id.Contains("ftt", StringComparison.InvariantCultureIgnoreCase)).ToList();
        assets.Content.Should().NotBeEmpty();
    }

}