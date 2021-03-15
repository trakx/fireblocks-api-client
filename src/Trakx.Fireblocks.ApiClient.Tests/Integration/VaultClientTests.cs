using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class VaultClientTests : FireblocksClientTestsBase
    {
        private readonly IVaultClient _fireblocksClient;

        public VaultClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture, output)
        {
            _fireblocksClient = ServiceProvider.GetRequiredService<IVaultClient>();
        }

        [Fact]
        public async Task AccountsAllAsync_should_return_all_accounts()
        {
            var response = await _fireblocksClient.AccountsAllAsync();
            response.Result.Should().NotBeNullOrEmpty();
            response.Result[0].Assets.Should().NotBeNullOrEmpty();
            response.Result[0].Assets[1].Id.Should().Be("ETH_TEST");
            Convert.ToDecimal(response.Result[0].Assets[1].Total).Should().BeGreaterOrEqualTo(0.1m);
        }
    }
}