using Trakx.Common.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

public class FireblocksApiClientPartialsTests
{
    [Fact]
    public void Ensure_all_api_clients_have_partial_method_PrepareRequest_implemented()
    {
        var apiClientInterfaces = ApiClientReflection.GetAllApiClientInterfaces(typeof(IFireblocksApiClientBase));

        foreach (var apiClientInterface in apiClientInterfaces)
        {
            var apiClientImplementation = ApiClientReflection.GetApiClientImplementation(apiClientInterface);
            if (apiClientImplementation == null)
                throw new NotImplementedException($"No implementation found for {apiClientInterface.Name}");

            var methods = apiClientImplementation.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var method = methods.FirstOrDefault(x => x.Name == "PrepareRequest");
            method.Should().NotBeNull("Failed to find PrepareRequest method in " + apiClientImplementation.Name);
        }
    }
}