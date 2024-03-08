namespace Trakx.Fireblocks.ApiClient.Utils;

/// <summary>
/// Interface for a class that can provide bearer credentials for an HTTP request.
/// </summary>
public interface IBearerCredentialsProvider
{
    /// <summary>
    /// Generate a JWT token for the given HTTP request message.
    /// </summary>
    public string GenerateJwtToken(HttpRequestMessage msg);
}