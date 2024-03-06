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
    private readonly FireblocksApiConfiguration _configuration;
    private readonly IBearerCredentialsProvider _bearerCredentialsProvider;
    private readonly CancellationTokenSource _tokenSource;

    private static readonly ILogger Logger = LoggerProvider.Create<ApiKeyCredentialsProvider>();

    /// <inheritdoc cref="IFireblocksCredentialsProvider" />
    public ApiKeyCredentialsProvider(FireblocksApiConfiguration configuration, IBearerCredentialsProvider bearerCredentialsProvider)
    {
        _configuration = configuration;
        _bearerCredentialsProvider = bearerCredentialsProvider;

        _tokenSource = new CancellationTokenSource();
    }


    #region Implementation of ICredentialsProvider

    /// <inheritdoc />
    public void AddCredentials(HttpRequestMessage msg)
    {
        var token = _bearerCredentialsProvider.GenerateJwtToken(msg);
        msg.Headers.Authorization = new AuthenticationHeaderValue(JwtScheme, token);
        msg.Headers.Add(ApiKeyHeader, _configuration.ApiPubKey);
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