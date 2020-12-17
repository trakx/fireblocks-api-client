using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Options;
using Serilog;

namespace Trakx.Fireblocks.ApiClient.Utils
{
    public class ApiKeyCredentialsProvider : ICredentialsProvider, IDisposable
    {
        internal const string ApiKeyHeader = "FIREBLOCKS-API-KEY";
        internal const string ApiNonceHeader = "FIREBLOCKS-API-NONCE";
        internal const string ApiSignatureHeader = "FIREBLOCKS-API-SIGNATURE";

        private readonly FireblocksApiConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly CancellationTokenSource _tokenSource;
        private readonly byte[] _encodingSecret;

        private static readonly ILogger Logger = Log.Logger.ForContext<MarketDataClient>();

        public ApiKeyCredentialsProvider(IOptions<FireblocksApiConfiguration> configuration, 
            IDateTimeProvider dateTimeProvider)
        {
            _configuration = configuration.Value;
            _dateTimeProvider = dateTimeProvider;

            _tokenSource = new CancellationTokenSource();
            _encodingSecret = Convert.FromBase64String(_configuration.ApiSecret);
        }

        
        #region Implementation of ICredentialsProvider

        /// <inheritdoc />
        public void AddCredentials(HttpRequestMessage msg)
        {
            var path = msg.RequestUri!.AbsolutePath;
            var method = msg.Method.Method.ToUpperInvariant();
            var nonce = GetNonce();
            var body = msg.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? string.Empty;

            var prehashString = path + method + nonce + body;
            Logger.Verbose("PreHash string is {prehashString}", prehashString);

            var devPrefix = msg.RequestUri.Host.Contains("dev-api") ? "DEV-" : string.Empty;

            msg.Headers.Add(devPrefix + ApiKeyHeader, _configuration.ApiKey);
            msg.Headers.Add(devPrefix + ApiNonceHeader, nonce);
            msg.Headers.Add(devPrefix + ApiSignatureHeader, GetSignature(prehashString));
            Logger.Verbose("Headers added");
        }
        #endregion

        private string GetNonce() => _dateTimeProvider.UtcNowAsOffset.ToUnixTimeMilliseconds()
            .ToString(CultureInfo.InvariantCulture);
        private string GetSignature(string preHash) => Convert.ToBase64String(new HMACSHA256(_encodingSecret)
            .ComputeHash(Encoding.UTF8.GetBytes(preHash)));
        
        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _tokenSource.Cancel();
            _tokenSource?.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}