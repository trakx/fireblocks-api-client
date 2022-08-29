using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

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
        Logger.Information("Found assets: {assets}", assets.Result);
        var asset = assets.Result.Where(i => i.Id.Contains("ftt", StringComparison.InvariantCultureIgnoreCase)).ToList();
        assets.Result.Should().NotBeEmpty();
    }

}
