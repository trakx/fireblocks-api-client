using System.Net.Http;

namespace Trakx.Fireblocks.ApiClient.Utils
{
    public interface IBearerCredentialsProvider
    {
        public string GenerateJwtToken(HttpRequestMessage msg);
    }
}