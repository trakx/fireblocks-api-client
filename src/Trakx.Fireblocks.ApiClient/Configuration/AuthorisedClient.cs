using System.Text;
using Trakx.Common.ApiClient;

namespace Trakx.Fireblocks.ApiClient;

internal abstract class AuthorisedClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    internal readonly ICredentialsProvider CredentialsProvider;
    protected string BaseUrl { get; }
    public virtual string HttpClientName { get; }

    protected AuthorisedClient(IClientConfigurator clientConfigurator)
    {
        _httpClientFactory = clientConfigurator.HttpClientFactory;
        HttpClientName = this.GetType().FullName!;
        CredentialsProvider = clientConfigurator.CredentialProvider;

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

    protected Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, string url, CancellationToken cancellationToken)
    {
        CredentialsProvider.AddCredentials(request);
        return Task.CompletedTask;
    }

    protected Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected Task ProcessResponseAsync(HttpClient client_, HttpResponseMessage response_, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}