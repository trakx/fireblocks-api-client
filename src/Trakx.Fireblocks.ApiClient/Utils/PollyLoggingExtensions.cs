using Microsoft.Extensions.Logging;
using Polly;

namespace Trakx.Fireblocks.ApiClient.Utils;
/// <summary>
/// Extensions methods for logging unexpected failures occuring when using an external API.
/// </summary>
public static class PollyLoggingExtensions
{
    /// <summary>
    /// Default method to log details of an API failure, usually after retrying the query using Polly.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> used to log the message.</param>
    /// <param name="result">The result of the API query.</param>
    /// <param name="timeSpan">Time the API took to return a response.</param>
    /// <param name="retryCount">The number of time the call was retried before being declared as a failure.</param>
    /// <param name="context">The Polly context in which the failure occured.</param>
    public static void LogApiFailure(this ILogger logger, DelegateResult<HttpResponseMessage> result, TimeSpan timeSpan, int retryCount, Context context)
    {
        if (result.Exception != null)
        {
            logger.LogWarning(result.Exception, "An exception occurred on retry {RetryAttempt} for {PolicyKey} - Retrying in {SleepDuration}ms",
                retryCount, context.PolicyKey, timeSpan.TotalMilliseconds);
        }
        else
        {
            logger.LogWarning("A non success code {StatusCode} with reason {Reason} and content {Content} was received on retry {RetryAttempt} for {PolicyKey} - Retrying in {SleepDuration}ms",
                (int)result.Result.StatusCode, result.Result.ReasonPhrase,
                result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult(),
                retryCount, context.PolicyKey, timeSpan.TotalMilliseconds);
        }
    }
}