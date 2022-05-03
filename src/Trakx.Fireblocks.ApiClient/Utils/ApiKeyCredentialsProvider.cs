using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Serilog;
using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient.Utils;

public interface IFireblocksCredentialsProvider : ICredentialsProvider { };

public class ApiKeyCredentialsProvider : IFireblocksCredentialsProvider, IDisposable
{
    internal const string ApiKeyHeader ="X-API-Key";
    internal const string JwtScheme = "Bearer";

    private readonly FireblocksApiConfiguration _configuration;
    private readonly IBearerCredentialsProvider _bearerCredentialsProvider;
    private readonly CancellationTokenSource _tokenSource;

    private static readonly ILogger Logger = Log.Logger.ForContext<ApiKeyCredentialsProvider>();

    public ApiKeyCredentialsProvider(IOptions<FireblocksApiConfiguration> configuration, IBearerCredentialsProvider bearerCredentialsProvider)
    {
        _configuration = configuration.Value;
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
        Logger.Verbose("Headers added");
    }

    public Task AddCredentialsAsync(HttpRequestMessage msg)
    {
        AddCredentials(msg);
        return Task.CompletedTask;
    }

    #endregion

    #region IDisposable

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