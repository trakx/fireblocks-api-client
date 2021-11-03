using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class UsersClientTests : FireblocksClientTestsBase
    {
        
        private readonly IUsersClient _fireblocksClient;
        private readonly MockCreator _mockCreator;
        
        public UsersClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output) 
            : base(apiFixture, output)
        {
            _mockCreator = new MockCreator(output);
            _fireblocksClient = ServiceProvider.GetRequiredService<Trakx.Fireblocks.ApiClient.IUsersClient>();
        }

        [Fact]
        public async Task GetUsersAsync_should_query_all_users()
        {
            var users = await _fireblocksClient.GetUsersAsync();
            users.Result.Users.Count.Should().BeGreaterOrEqualTo(2);
        }
        
    }
}