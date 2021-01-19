using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
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
        }


        private static string GetCurrentOpenApi()
        {
            var directory = GetAssemblyDirectory();
            var openApiPath = Path.Combine(directory?.Parent?.Parent?.Parent?.Parent?.ToString() ?? "", "Trakx.Fireblocks.ApiClient\\OpenApi3.yaml");
            return File.ReadAllText(openApiPath);
        }

        private static DirectoryInfo? GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().Location;
            return new DirectoryInfo(codeBase).Parent;
        }
    }


}
