
namespace Trakx.Fireblocks.ApiClient
{
    internal partial class AccountsClient
    {
#pragma warning disable S1172 // Unused method parameters should be removed
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            CredentialProvider.AddCredentials(request);
        }
    }

}