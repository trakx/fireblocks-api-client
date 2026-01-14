using System.Text;
using Trakx.Common.ApiClient;

namespace Trakx.Fireblocks.ApiClient;

internal abstract class AuthorisedClient(IClientConfigurator clientConfigurator)
{
    internal readonly ICredentialsProvider CredentialsProvider = clientConfigurator.CredentialProvider;

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