using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class VaultClientTests : FireblocksClientTestsBase
    {
        private readonly IVaultClient _vaultClient;

        public VaultClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) 
            : base(apiFixture, output)
        {
            _vaultClient = ServiceProvider.GetRequiredService<IVaultClient>();
        }

        [Fact]
        public async Task GetVaultAccountsAsync_should_return_all_vault_accounts()
        {
            var response = await _vaultClient.GetVaultAccountsAsync();
            response.Result.Count.Should().BeGreaterOrEqualTo(2);
        }
    }
}