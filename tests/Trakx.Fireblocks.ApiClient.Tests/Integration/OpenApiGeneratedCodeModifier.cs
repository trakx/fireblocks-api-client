namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class OpenApiGeneratedCodeModifier : Trakx.Common.Testing.Documentation.OpenApiGeneratedCodeModifier
{
    public OpenApiGeneratedCodeModifier(ITestOutputHelper output)
        : base(output)
    {
        FilePaths.Add(ProjectPaths.ModelsFile);
    }
}