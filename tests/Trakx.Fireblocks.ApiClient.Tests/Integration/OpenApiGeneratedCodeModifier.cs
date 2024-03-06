using FluentAssertions.Execution;
using Trakx.Common.Infrastructure.Environment.Env;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration;

public class OpenApiGeneratedCodeModifier : Trakx.Common.Testing.Documentation.OpenApiGeneratedCodeModifier
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