using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Trakx.Common.ApiClient;
using Trakx.Common.Logging;

namespace Trakx.Fireblocks.ApiClient.Utils;

/// <summary>
/// Interface for a class that can provide credentials for Fireblocks API.
/// </summary>
public interface IFireblocksCredentialsProvider : ICredentialsProvider { };

/// <inheritdoc cref="IFireblocksCredentialsProvider" />
public class ApiKeyCredentialsProvider : IFireblocksCredentialsProvider, IDisposable
{
    internal const string ApiKeyHeader = "X-API-Key";
    internal const string JwtScheme = "Bearer";
    internal readonly FireblocksApiCredentialsConfiguration ApiCredentialsConfiguration;
    internal readonly IBearerCredentialsProvider BearerCredentialsProvider;
    private readonly CancellationTokenSource _tokenSource;

    private static readonly ILogger Logger = LoggerProvider.Create<ApiKeyCredentialsProvider>();

    /// <summary>
    /// Constructor to use the default <see cref="FireblocksApiCredentialsConfiguration"/>.
    /// </summary>
    public ApiKeyCredentialsProvider(FireblocksApiConfiguration apiConfiguration, IBearerCredentialsProvider bearerCredentialsProvider)
        : this((FireblocksApiCredentialsConfiguration)apiConfiguration, bearerCredentialsProvider) { }

    /// <summary>
    /// Constructor to use a custom <see cref="FireblocksApiCredentialsConfiguration"/>.
    /// </summary>
    public ApiKeyCredentialsProvider(FireblocksApiCredentialsConfiguration apiCredentialsConfiguration, IBearerCredentialsProvider bearerCredentialsProvider)
    {
        ApiCredentialsConfiguration = apiCredentialsConfiguration;
        BearerCredentialsProvider = bearerCredentialsProvider;
        _tokenSource = new CancellationTokenSource();
    }

    #region Implementation of ICredentialsProvider

    /// <inheritdoc />
    public void AddCredentials(HttpRequestMessage msg)
    {
        var token = BearerCredentialsProvider.GenerateJwtToken(msg);
        msg.Headers.Authorization = new AuthenticationHeaderValue(JwtScheme, token);
        msg.Headers.Add(ApiKeyHeader, ApiCredentialsConfiguration.ApiPubKey);
        Logger.LogTrace("Headers added");
    }

    /// <inheritdoc />
    public Task AddCredentialsAsync(HttpRequestMessage msg)
    {
        AddCredentials(msg);
        return Task.CompletedTask;
    }

    #endregion

    #region IDisposable

    /// <inheritdoc cref="IDisposable.Dispose" />
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _tokenSource.Cancel();
        _tokenSource.Dispose();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}