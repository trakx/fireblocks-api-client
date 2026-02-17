using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Fireblocks.ApiClient.Tests.Integration.Base;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public partial class VaultClientTests : FireblocksClientTestsBase
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
        var response = await _vaultClient.GetPagedVaultAccountsAsync();
        response.Content.Should().NotBeNull();
        var accounts = response.Content.Accounts;
        accounts.Should().NotBeNullOrEmpty();
        accounts.Should().Contain(x => x.Assets.Any(x => x.Id == "BTC_TEST"));
    }

    [Fact]
    public async Task List_vault_accounts_with_details()
    {
        var userIdPattern = UserIdVaultNameRegex();
        string? after = null;

        do
        {
            var response = await _vaultClient.GetPagedVaultAccountsAsync(after: after, limit: 200);
            var page = response.Content;

            foreach (var vault in page.Accounts)
            {
                if (userIdPattern.IsMatch(vault.Name ?? "")) continue;

                _logger.Information("Vault: Id={Id}, Name={Name}, AutoFuel={AutoFuel}, Hidden={Hidden}",
                    vault.Id, vault.Name, vault.AutoFuel, vault.HiddenOnUI);

                if (vault.Assets is not { Count: > 0 }) continue;
                foreach (var asset in vault.Assets)
                {
                    _logger.Information("  Asset: Id={AssetId}, Total={Total}, Available={Available}, Pending={Pending}, Frozen={Frozen}",
                        asset.Id, asset.Total, asset.Available, asset.Pending, asset.Frozen);
                }
            }

            after = page.Paging?.After;
        } while (!string.IsNullOrEmpty(after));
    }

    [Fact]
    public async Task GetVaultAccountsAsync_is_case_insensitive()
    {
        var response = await _vaultClient.GetPagedVaultAccountsAsync(namePrefix: "exchange");
        response.Content.Should().NotBeNull();
        var accounts = response.Content.Accounts;
        accounts.Should().NotBeNullOrEmpty();
        accounts.Should().Contain(x => x.Name == "Exchange Warm Wallet");
    }

    [GeneratedRegex(@"^user__ID[0-9A-Fa-f]{10}$")]
    private static partial Regex UserIdVaultNameRegex();
}
