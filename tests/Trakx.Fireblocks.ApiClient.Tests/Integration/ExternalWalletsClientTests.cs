using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Testing.Mocks;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class ExternalWalletsClientTests : FireblocksClientTestsBase
{
    private readonly IExternal_walletsClient _externalWalletsClient;
    private readonly MockCreator _mockCreator;

    public ExternalWalletsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _externalWalletsClient = _serviceProvider.GetRequiredService<IExternal_walletsClient>();
        _mockCreator = new MockCreator(output);
    }

    [Fact]
    public async Task GetExternalWalletsAsync_should_return_all_wallets()
    {
        var response = await _externalWalletsClient.External_walletsAllAsync();
        response.Content.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetExternalWalletAsync_should_return_one_particular_wallet()
    {
        var response = await _externalWalletsClient.External_walletsGETAsync("60af415a-61df-59d5-de2b-580cebc097fc", CancellationToken.None);
        response.Content.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateExternalWalletAsync_should_add_a_new_wallet_in_fireblocks_database()
    {
        var walletName = _mockCreator.GetString(10);
        var refId = _mockCreator.GetString(10);
        var response = await _externalWalletsClient.External_walletsPOSTAsync(new Body21
        {
            Name = walletName,
            CustomerRefId = refId
        });
        var walletId = response.Content.Id;

        // After write op, fireblocks needs some time to refresh data :(
        await Task.Delay(2000);

        var response2 = await _externalWalletsClient.External_walletsGETAsync(walletId);
        response2.Content.Name.Should().Be(walletName);
        await _externalWalletsClient.External_walletsDELETEAsync(walletId);

        // After write op, fireblocks needs some time to refresh data :(
        await Task.Delay(2000);

        await new Func<Task>(async () => await _externalWalletsClient.External_walletsGETAsync(walletId)).Should()
            .ThrowAsync<ApiException>();
    }
}