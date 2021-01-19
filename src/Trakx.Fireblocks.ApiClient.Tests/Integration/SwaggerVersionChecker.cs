using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Trakx.Utils.Extensions;
using Xunit;

namespace Trakx.Fireblocks.ApiClient.Tests.Integration
{
    public class SwaggerVersionChecker
    {
        private readonly IFlurlClient _fireblocksClient;

        public SwaggerVersionChecker()
        {
            _fireblocksClient = new FlurlClient("https://api.fireblocks.io");
        }

        [Fact]
        public async Task VerifyOpenApiVersion()
        {
            var apiResponse = await _fireblocksClient.Request("docs", "v1", "swagger").SendAsync(HttpMethod.Get);
            var fireblocksOpenApi = await apiResponse.GetStringAsync();
            var currentOpenApi = GetCurrentOpenApi();

            var fireblocksRawOpenApi = Regex.Replace(fireblocksOpenApi, @"\s+", string.Empty);
            var currentRawOpenApi = Regex.Replace(currentOpenApi, @"\s+", string.Empty);

            var isEqual = string.Equals(fireblocksRawOpenApi, currentRawOpenApi, StringComparison.OrdinalIgnoreCase);
            isEqual.Should().BeTrue();
            _fireblocksClient.Dispose();
        }


        private static string GetCurrentOpenApi()
        {
            var isRootDirectory = DirectoryInfoExtensions.TryWalkBackToRepositoryRoot(null, out var rootDirectory);
            if (!isRootDirectory || rootDirectory == null)
                return "";

            var openApiPath = Path.Combine(rootDirectory.ToString(), "src", "Trakx.Fireblocks.ApiClient", "openApi3.yaml");
            return File.ReadAllText(openApiPath);
        }
    }
}
