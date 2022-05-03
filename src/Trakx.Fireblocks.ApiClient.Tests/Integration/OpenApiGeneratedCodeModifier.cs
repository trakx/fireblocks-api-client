using FluentAssertions.Execution;
using Trakx.Utils.Extensions;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class OpenApiGeneratedCodeModifier : Trakx.Utils.Testing.OpenApiGeneratedCodeModifier
{
    public OpenApiGeneratedCodeModifier(ITestOutputHelper output)
        : base(output)
    {
        var foundRoot = default(DirectoryInfo).TryWalkBackToRepositoryRoot(out var rootDirectory)!;
        if (!foundRoot) throw new AssertionFailedException("Failed to retrieve repository root.");
        FilePaths.Add(Path.Combine(rootDirectory!.FullName, "src",
            "Trakx.Fireblocks.ApiClient", "ApiClients.cs"));
    }
}