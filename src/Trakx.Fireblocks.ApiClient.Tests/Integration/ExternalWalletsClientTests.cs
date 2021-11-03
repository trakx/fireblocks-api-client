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
        private readonly IExternalWalletsClient _client;
        private readonly MockCreator _mockCreator;

        public ExternalWalletsClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) : base(apiFixture,
            output)
        {
            _client = ServiceProvider.GetRequiredService<IExternalWalletsClient>();
            _mockCreator = new MockCreator(output);
        }

        [Fact]
        public async Task GetExternalWalletsAsync_should_return_all_wallets()
        {
            var response = await _client.GetExternalWalletsAsync();
            response.Result.Count.Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task GetExternalWalletAsync_should_return_one_particular_wallet()
        {
            var response = await _client.GetExternalWalletAsync("001", CancellationToken.None);
            response.Result.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateExternalWalletAsync_should_add_a_new_wallet_in_fireblocks_database()
        {
            var walletName = _mockCreator.GetRandomString(10);
            var refId = _mockCreator.GetRandomString(10);
            var response = await _client.CreateExternalWalletAsync(new Body12
            {
                Name = walletName,
                CustomerRefId = refId
            });
            var walletId = response.Result.Id;
            var response2 = await _client.GetExternalWalletAsync(walletId);
            response2.Result.Name.Should().Be(walletName);
            response2.Result.CustomerRefId.Should().Be(refId);
            await _client.DeleteExternalWalletByIdAsync(walletId);
            await new Func<Task>(async () => await _client.GetExternalWalletAsync(walletId)).Should()
                .ThrowAsync<ApiException>();
        }
    }
}