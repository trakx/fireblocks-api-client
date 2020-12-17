using System;
using System.IO;

namespace Trakx.Fireblocks.ApiClient.Tests
{
    public static class Secrets
    {
        static Secrets()
        {
            var srcPath = new DirectoryInfo(Environment.CurrentDirectory).Parent?.Parent?.Parent?.Parent;
            try
            {
                DotNetEnv.Env.Load(Path.Combine(srcPath?.FullName ?? string.Empty, ".env"));
            }
            catch (Exception)
            {
                // Fail to load the file on the CI pipeline, it should have environment variables defined.
            }
        }

        public static string FireblocksApiKey => Environment.GetEnvironmentVariable("FireblocksApiConfiguration__ApiKey")!;
        public static string FireblocksApiSecret => Environment.GetEnvironmentVariable("FireblocksApiConfiguration__ApiSecret")!;
    }
    
}