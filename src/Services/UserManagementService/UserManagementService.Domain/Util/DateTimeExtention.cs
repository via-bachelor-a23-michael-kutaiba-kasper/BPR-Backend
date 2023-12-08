using System.Globalization;

namespace UserManagementService.Domain.Util;

public static class DateTimeExtension
{
    public static int GetIso8601WeekNumber(this DateTimeOffset date, CultureInfo culture)
    {
        Calendar calendar = culture.Calendar;

        // Determine the week of the year according to ISO 8601
        int weekNumber = calendar.GetWeekOfYear(date.DateTime, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);

        return weekNumber;
    }
}