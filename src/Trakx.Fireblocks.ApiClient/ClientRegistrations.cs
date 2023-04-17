using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Trakx.Common.Logging;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient;

public static partial class AddFireblocksClientExtension
{
    private static void AddClients(this IServiceCollection services)
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(100), retryCount: 2, fastFirst: true);

        services.AddHttpClient<IAudit_LogsClient, Audit_LogsClient>("Trakx.Fireblocks.ApiClient.Audit_LogsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Audit_LogsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Audit_LogsClient"));


        services.AddHttpClient<IContractsClient, ContractsClient>("Trakx.Fireblocks.ApiClient.ContractsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<ContractsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("ContractsClient"));


        services.AddHttpClient<IExchange_accountsClient, Exchange_accountsClient>("Trakx.Fireblocks.ApiClient.Exchange_accountsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Exchange_accountsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Exchange_accountsClient"));


        services.AddHttpClient<IExternal_walletsClient, External_walletsClient>("Trakx.Fireblocks.ApiClient.External_walletsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<External_walletsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("External_walletsClient"));


        services.AddHttpClient<IFee_payerClient, Fee_payerClient>("Trakx.Fireblocks.ApiClient.Fee_payerClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Fee_payerClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Fee_payerClient"));


        services.AddHttpClient<IFiat_accountsClient, Fiat_accountsClient>("Trakx.Fireblocks.ApiClient.Fiat_accountsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Fiat_accountsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Fiat_accountsClient"));


        services.AddHttpClient<IGas_stationsClient, Gas_stationsClient>("Trakx.Fireblocks.ApiClient.Gas_stationsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Gas_stationsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Gas_stationsClient"));


        services.AddHttpClient<IInternal_walletsClient, Internal_walletsClient>("Trakx.Fireblocks.ApiClient.Internal_walletsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Internal_walletsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Internal_walletsClient"));


        services.AddHttpClient<INetwork_connectionClient, Network_connectionClient>("Trakx.Fireblocks.ApiClient.Network_connectionClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Network_connectionClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Network_connectionClient"));


        services.AddHttpClient<INetwork_connectionsClient, Network_connectionsClient>("Trakx.Fireblocks.ApiClient.Network_connectionsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Network_connectionsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Network_connectionsClient"));


        services.AddHttpClient<IOff_exchangesClient, Off_exchangesClient>("Trakx.Fireblocks.ApiClient.Off_exchangesClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Off_exchangesClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Off_exchangesClient"));


        services.AddHttpClient<IPaymentsClient, PaymentsClient>("Trakx.Fireblocks.ApiClient.PaymentsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<PaymentsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("PaymentsClient"));


        services.AddHttpClient<ISupported_assetsClient, Supported_assetsClient>("Trakx.Fireblocks.ApiClient.Supported_assetsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Supported_assetsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Supported_assetsClient"));


        services.AddHttpClient<ITransactionsClient, TransactionsClient>("Trakx.Fireblocks.ApiClient.TransactionsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<TransactionsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("TransactionsClient"));


        services.AddHttpClient<ITransfer_ticketsClient, Transfer_ticketsClient>("Trakx.Fireblocks.ApiClient.Transfer_ticketsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<Transfer_ticketsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("Transfer_ticketsClient"));


        services.AddHttpClient<IUsersClient, UsersClient>("Trakx.Fireblocks.ApiClient.UsersClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<UsersClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("UsersClient"));


        services.AddHttpClient<IVaultsClient, VaultsClient>("Trakx.Fireblocks.ApiClient.VaultsClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<VaultsClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("VaultsClient"));


        services.AddHttpClient<IWebhooksClient, WebhooksClient>("Trakx.Fireblocks.ApiClient.WebhooksClient")
            .AddPolicyHandler((s, request) =>
                Policy<HttpResponseMessage>
                .Handle<ApiException>()
                .Or<HttpRequestException>()
                .OrTransientHttpStatusCode()
                .WaitAndRetryAsync(delay,
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        var logger = LoggerProvider.Create<WebhooksClient>();
                        logger.LogApiFailure(result, timeSpan, retryCount, context);
                    })
                .WithPolicyKey("WebhooksClient"));


    }
}
