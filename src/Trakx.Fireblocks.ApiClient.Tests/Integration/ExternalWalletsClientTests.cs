using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class ExternalWalletsClientTests : FireblocksClientTestsBase
    {
        private readonly IExternalWalletsClient _externalWalletsClient;
        private readonly MockCreator _mockCreator;

        public ExternalWalletsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture,
            output)
        {
            _externalWalletsClient = ServiceProvider.GetRequiredService<IExternalWalletsClient>();
            _mockCreator = new MockCreator(output);
        }

        [Fact]
        public async Task GetExternalWalletsAsync_should_return_all_wallets()
        {
            var response = await _externalWalletsClient.GetExternalWalletsAsync();
            response.Result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetExternalWalletAsync_should_return_one_particular_wallet()
        {
            var response = await _externalWalletsClient.GetExternalWalletAsync("60af415a-61df-59d5-de2b-580cebc097fc", CancellationToken.None);
            response.Result.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateExternalWalletAsync_should_add_a_new_wallet_in_fireblocks_database()
        {
            var walletName = _mockCreator.GetRandomString(10);
            var refId = _mockCreator.GetRandomString(10);
            var response = await _externalWalletsClient.CreateExternalWalletAsync(new Body12
            {
                Name = walletName,
                CustomerRefId = refId
            });
            var walletId = response.Result.Id;
            
            // After write op, fireblocks needs some time to refresh data :(
            Thread.Sleep(2000);

            var response2 = await _externalWalletsClient.GetExternalWalletAsync(walletId);
            response2.Result.Name.Should().Be(walletName);
            await _externalWalletsClient.DeleteExternalWalletByIdAsync(walletId);

            // After write op, fireblocks needs some time to refresh data :(
            Thread.Sleep(2000);

            await new Func<Task>(async () => await _externalWalletsClient.GetExternalWalletAsync(walletId)).Should()
                .ThrowAsync<ApiException>();
        }
    }
}