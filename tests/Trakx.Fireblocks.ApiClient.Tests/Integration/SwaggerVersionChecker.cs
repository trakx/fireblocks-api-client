using Flurl.Http;
using Trakx.Common.Infrastructure.Environment.Env;
using YamlDotNet.RepresentationModel;

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

        var getCurrentOpenApiDescription = await _fireblocksClient.Request("api", "v1", "swagger").SendAsync(HttpMethod.Get);
        var fireblocksOpenApi = await getCurrentOpenApiDescription.GetStringAsync();
        var latestVersion = GetVersion(fireblocksOpenApi);

        var localOpenApi = GetCurrentOpenApiContent();
        var localVersion = GetVersion(localOpenApi);

        localVersion.Should().NotBeNullOrWhiteSpace();

        localVersion.Should().Be(latestVersion,
            $"the latest api from fireblocks (v{latestVersion}) should be merged with the current local (v{localVersion}) to integrate latest changes.");
    }

    private static string GetVersion(string yaml)
    {
        using (var reader = new StringReader(yaml))
        {
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);

            var mapping =
                (YamlMappingNode)yamlStream.Documents[0].RootNode;

            var info = (YamlMappingNode)mapping.Children[new YamlScalarNode("info")];
            var version = ((YamlScalarNode)info.Children[new YamlScalarNode("version")]).Value;

            return version!;
        }
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