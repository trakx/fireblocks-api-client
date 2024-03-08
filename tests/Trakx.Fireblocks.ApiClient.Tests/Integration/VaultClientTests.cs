using Microsoft.Extensions.DependencyInjection;
using Trakx.Fireblocks.ApiClient.Tests.Integration.Base;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class VaultClientTests : FireblocksClientTestsBase
{
    private readonly IVaultsClient _vaultClient;

    public VaultClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _vaultClient = _serviceProvider.GetRequiredService<IVaultsClient>();
    }

    [Fact]
    public async Task GetVaultAccountsAsync_should_return_all_vault_accounts()
    {
        var response = await _vaultClient.Accounts_pagedAsync();
        response.Content.Should().NotBeNull();
        var accounts = response.Content.Accounts;
        accounts.Should().NotBeNullOrEmpty();
        accounts[0].Assets.Should().NotBeNullOrEmpty();
        accounts.Should().Contain(x => x.Assets.Any(x => x.Id == "BTC_TEST"));
    }
}