using System;

namespace Trakx.Fireblocks.ApiClient.Utils
{
    /// <summary>
    /// Allows easier testing, by setting fixed return values.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTimeOffset UtcNowAsOffset { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        #region Implementation of IDateTimeProvider

        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset UtcNowAsOffset => DateTimeOffset.UtcNow;
        #endregion
    }
}