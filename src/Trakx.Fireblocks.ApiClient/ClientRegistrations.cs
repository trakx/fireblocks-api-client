using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Serilog;
using Trakx.Utils.Apis;

namespace Trakx.Fireblocks.ApiClient;

    public static partial class AddFireblocksClientExtension
    {
        private static void AddClients(this IServiceCollection services)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(100), retryCount: 2, fastFirst: true);
            
            services.AddHttpClient<IVaultClient, VaultClient>("Trakx.Fireblocks.ApiClient.VaultClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<VaultClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("VaultClient"));

        
            services.AddHttpClient<IInternalWalletsClient, InternalWalletsClient>("Trakx.Fireblocks.ApiClient.InternalWalletsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<InternalWalletsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("InternalWalletsClient"));

        
            services.AddHttpClient<IExternalWalletsClient, ExternalWalletsClient>("Trakx.Fireblocks.ApiClient.ExternalWalletsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<ExternalWalletsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("ExternalWalletsClient"));

        
            services.AddHttpClient<IExchangeAccountsClient, ExchangeAccountsClient>("Trakx.Fireblocks.ApiClient.ExchangeAccountsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<ExchangeAccountsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("ExchangeAccountsClient"));

        
            services.AddHttpClient<IFiatAccountsClient, FiatAccountsClient>("Trakx.Fireblocks.ApiClient.FiatAccountsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<FiatAccountsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("FiatAccountsClient"));

        
            services.AddHttpClient<ITransactionsClient, TransactionsClient>("Trakx.Fireblocks.ApiClient.TransactionsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<TransactionsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("TransactionsClient"));

        
            services.AddHttpClient<ISupportedAssetsClient, SupportedAssetsClient>("Trakx.Fireblocks.ApiClient.SupportedAssetsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<SupportedAssetsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("SupportedAssetsClient"));

        
            services.AddHttpClient<INetworkConnectionsClient, NetworkConnectionsClient>("Trakx.Fireblocks.ApiClient.NetworkConnectionsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<NetworkConnectionsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("NetworkConnectionsClient"));

        
            services.AddHttpClient<ITransferTicketsClient, TransferTicketsClient>("Trakx.Fireblocks.ApiClient.TransferTicketsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<TransferTicketsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("TransferTicketsClient"));

        
            services.AddHttpClient<IFeeClient, FeeClient>("Trakx.Fireblocks.ApiClient.FeeClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<FeeClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("FeeClient"));

        
            services.AddHttpClient<IWebhooksClient, WebhooksClient>("Trakx.Fireblocks.ApiClient.WebhooksClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<WebhooksClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("WebhooksClient"));

        
            services.AddHttpClient<IUsersClient, UsersClient>("Trakx.Fireblocks.ApiClient.UsersClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<UsersClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("UsersClient"));

        
            services.AddHttpClient<IOffExchangeAccountsClient, OffExchangeAccountsClient>("Trakx.Fireblocks.ApiClient.OffExchangeAccountsClient")
                .AddPolicyHandler((s, request) =>
                    Policy<HttpResponseMessage>
                    .Handle<ApiException>()
                    .Or<HttpRequestException>()
                    .OrTransientHttpStatusCode()
                    .WaitAndRetryAsync(delay,
                        onRetry: (result, timeSpan, retryCount, context) =>
                        {
                            var logger = Log.Logger.ForContext<OffExchangeAccountsClient>();
                            logger.LogApiFailure(result, timeSpan, retryCount, context);
                        })
                    .WithPolicyKey("OffExchangeAccountsClient"));

        
    }
}
