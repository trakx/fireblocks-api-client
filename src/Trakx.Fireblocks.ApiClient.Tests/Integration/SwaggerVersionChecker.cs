using System.Text.RegularExpressions;
using FluentAssertions;
using Flurl.Http;
using Trakx.Common.Infrastructure.Environment.Env;
using Xunit;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class SwaggerVersionChecker : IDisposable
{
    private readonly IFlurlClient _fireblocksClient;

    public SwaggerVersionChecker()
    {
        _fireblocksClient = new FlurlClient("https://docs.fireblocks.com");
    }

   // [Fact(Skip = "local openapi file was changed to improve methods signature and fix nullable issues.
   // After that, this test is no longer working, as it takes the assumption that both files, local and remote, are identical.")]
    [Fact]
    public async Task VerifyOpenApiVersion()
    {
        const string regexPattern = @"info:\r?\n\s+title(.+(\r?\n))\s+version:\s\""(?<version>[0-9\.]+)\""";
        var versionRegex = new Regex(regexPattern, RegexOptions.Multiline);

        var getCurrentOpenApiDescription = await _fireblocksClient.Request("api", "v1", "swagger").SendAsync(HttpMethod.Get);
        var fireblocksOpenApi = await getCurrentOpenApiDescription.GetStringAsync();

        var latestVersion = versionRegex.Match(fireblocksOpenApi).Groups["version"].Value;

        var localOpenApi = GetCurrentOpenApiContent();
        var localVersion = versionRegex.Match(localOpenApi).Groups["version"].Value;

        localVersion.Should().NotBeNullOrWhiteSpace();

        localVersion.Should().Be(latestVersion,
            $"the latest api from fireblocks (v{latestVersion}) should be merged with the current local (v{localVersion}) to integrate latest changes.");
    }


    private static string GetCurrentOpenApiContent()
    {
        var isRootDirectory = EnvExtensions.TryWalkBackToRepositoryRoot(null, out var rootDirectory);
        if (!isRootDirectory || rootDirectory == null)
            return "";

        var openApiPath = Path.Combine(rootDirectory.ToString(), "src", "Trakx.Fireblocks.ApiClient", "openApi3.yaml");
        return File.ReadAllText(openApiPath);
    }

    public void Dispose()
    {
        _fireblocksClient.Dispose();
    }
}
