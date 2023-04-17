using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class VaultClientTests : FireblocksClientTestsBase
{
    private readonly IVaultsClient _vaultClient;

    public VaultClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _vaultClient = ServiceProvider.GetRequiredService<IVaultsClient>();
    }

    [Fact]
    public async Task GetVaultAccountsAsync_should_return_all_vault_accounts()
    {
        var response = await _vaultClient.AccountsAllAsync();
        response.Content.Should().NotBeNullOrEmpty();
        response.Content[0].Assets.Should().NotBeNullOrEmpty();
        response.Content[0].Assets.Should().Contain(x => x.Id == "BTC_TEST");
    }
}
