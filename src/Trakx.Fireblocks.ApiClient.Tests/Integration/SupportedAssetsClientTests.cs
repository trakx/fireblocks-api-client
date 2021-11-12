using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class SupportedAssetsClientTests : FireblocksClientTestsBase
    {
        
        private readonly ISupportedAssetsClient _supportedAssetsClient;
        
        public SupportedAssetsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) 
            : base(apiFixture, output)
        {
            _supportedAssetsClient = ServiceProvider.GetRequiredService<ISupportedAssetsClient>();
        }

        [Fact]
        public async Task GetSupportedAssetsAsync_should_query_all_assets()
        {
            var assets = await _supportedAssetsClient.GetSupportedAssetsAsync();
            assets.Result.Should().NotBeEmpty();
        }
        
    }
}