using FluentAssertions.Execution;
using Trakx.Common.Infrastructure.Environment.Env;

namespace Trakx.Fireblocks.ApiClient.Tests;

internal static class ProjectPaths
{
    internal static string ProjectRoot { get; }
    internal static string ProjectName { get; }
    internal static string CodeRoot => Path.Combine(ProjectRoot, "src", ProjectName);
    internal static string ClientsFile => Path.Combine(CodeRoot, "FireblocksApiClients.cs");
    internal static string InterfacesFile => Path.Combine(CodeRoot, "FireblocksApiInterfacesAndModels.cs");
    internal static string ModelsFile => InterfacesFile;

    static ProjectPaths()
    {
        var foundRoot = default(DirectoryInfo).TryWalkBackToRepositoryRoot(out var rootDirectory)!;
        if (!foundRoot) throw new AssertionFailedException("Failed to retrieve repository root.");

        ProjectRoot = rootDirectory!.FullName;
        ProjectName = typeof(AuthorisedClient).Assembly.GetName().Name!;
    }
}
