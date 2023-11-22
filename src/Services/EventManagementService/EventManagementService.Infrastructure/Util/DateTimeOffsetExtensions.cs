namespace EventManagementService.Infrastructure.Util;

public static class DateTimeOffsetExtensions
{
    public static string ToFormattedUtcString(this DateTimeOffset dt)
    {
        return dt.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
    }
}