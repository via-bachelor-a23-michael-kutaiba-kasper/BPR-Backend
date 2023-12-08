using System.Globalization;

namespace UserManagementService.Infrastructure.Util;

public static class DateTimeOffsetExtensions
{
    public static string ToFormattedUtcString(this DateTimeOffset dt)
    {
        return dt.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }

    public static DateTimeOffset Truncate(this DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
    {
        long ticks = dateTimeOffset.UtcDateTime.Ticks - (dateTimeOffset.UtcDateTime.Ticks % timeSpan.Ticks);
        return new DateTimeOffset(new DateTime(ticks, DateTimeKind.Utc), TimeSpan.Zero);
    }
}