using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class FireBlocksClientAccountTests : FireblocksClientTestsBase
    {
        private readonly IFireblocksClient _fireblocksClient;

        public FireBlocksClientAccountTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture, output)
        {
            _fireblocksClient = ServiceProvider.GetRequiredService<IFireblocksClient>();
        }

        [Fact]
        public async Task Exchange_accountsAllAsync_should_return_all_accounts()
        {
            var response = await _fireblocksClient.Exchange_accountsAllAsync();
            response.Result.Should().NotBeNullOrEmpty();
        }
    }
}
