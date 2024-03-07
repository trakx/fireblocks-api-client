using Trakx.Common.ApiClient;

namespace Trakx.Fireblocks.ApiClient;

internal abstract class AuthorisedClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    internal readonly ICredentialsProvider CredentialProvider;
    protected string BaseUrl { get; }
    public virtual string HttpClientName { get; }

    protected AuthorisedClient(IClientConfigurator clientConfigurator)
    {
        _httpClientFactory = clientConfigurator.HttpClientFactory;
        HttpClientName = this.GetType().FullName!;
        CredentialProvider = clientConfigurator.CredentialProvider;

        BaseUrl = clientConfigurator.ApiClientConfiguration.BaseUrl.AbsoluteUri;
        if (BaseUrl[^1] != '/') BaseUrl += "/";
    }

    /// <summary>
    /// Create a new <see cref="HttpClient"/>.
    /// </summary>
    public Task<HttpClient> CreateHttpClientAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_httpClientFactory.CreateClient(HttpClientName));
    }

    /// <summary>
    /// Create a new <see cref="HttpRequestMessage"/> with the credentials added.
    /// </summary>
    protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
    {
        HttpRequestMessage httpRequestMessage = new();
        CredentialProvider.AddCredentials(httpRequestMessage);
        return Task.FromResult(httpRequestMessage);
    }
}