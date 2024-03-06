using Microsoft.Extensions.DependencyInjection;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class UsersClientTests : FireblocksClientTestsBase
{

    private readonly IUsersClient _usersClient;

    public UsersClientTests(FireblocksApiFixture apiFixture, ITestOutputHelper output)
        : base(apiFixture, output)
    {
        _usersClient = _serviceProvider.GetRequiredService<IUsersClient>();
    }

    [Fact(Skip = "Fireblocks api is always returning 429 - too many requests for this end-point.")]
    public async Task GetUsersAsync_should_query_all_users()
    {
        var users = await _usersClient.UsersAsync();
        users.Content.Users.Should().NotBeEmpty();
    }

}