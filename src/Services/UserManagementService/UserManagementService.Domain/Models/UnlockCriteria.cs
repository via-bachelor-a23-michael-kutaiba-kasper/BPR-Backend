using System.Globalization;
using UserManagementService.Domain.Util;

namespace UserManagementService.Domain.Models;

public abstract class UnlockCriteria
{
    public IReadOnlyCollection<UnlockableProgress> UnlockableProgresses { get; set; } = new List<UnlockableProgress>();
    public long Goal { get; set; }
    public long ExpReward { get; set; }

    public abstract bool hasBeenMet();
}

public class CategoryAttendanceCriteria : UnlockCriteria
{
    public Category Type { get; set; }

    public override bool hasBeenMet()
    {
        return Goal <= UnlockableProgresses.Count;
    }
}

public class MonthlyAttendanceCriteria : UnlockCriteria
{
    private DateTimeOffset MonthOfYear;

    public override bool hasBeenMet()
    {
        var eventAttendentInMonth = 0;

        if (!UnlockableProgresses.Any())
        {
            return false;
        }
        var currentMonth = UnlockableProgresses.First().Date.Month;
        
        foreach (var progress in UnlockableProgresses)
        {
            if (progress.Date.Month != currentMonth)
            {
                eventAttendentInMonth = 1;
                currentMonth = progress.Date.Month;
                continue;
            }

            if (eventAttendentInMonth == Goal)
            {
                return true;
            }
            eventAttendentInMonth++;
        }

        return false;
    }
}


public class WeeklyAttendanceCriteria : UnlockCriteria
{
    private DateTimeOffset WeekOfMonthOfYear;

    public override bool hasBeenMet()
    {
        var eventAttendentInWeek = 0;

        if (!UnlockableProgresses.Any())
        {
            return false;
        }
        var culture = CultureInfo.CurrentCulture;
        var currentWeek = UnlockableProgresses.First().Date.GetIso8601WeekNumber(culture);
        
        foreach (var progress in UnlockableProgresses)
        {
            if (progress.Date.GetIso8601WeekNumber(culture) != currentWeek)
            {
                eventAttendentInWeek = 1;
                currentWeek = progress.Date.GetIso8601WeekNumber(culture);
                continue;
            }

            if (eventAttendentInWeek == Goal)
            {
                return true;
            }
            eventAttendentInWeek++;
        }

        return false;
    }
}


public class MonthlyExpGoalUnlockCriteria : UnlockCriteria
{
    public long CurrentExp { get; set; }
    
    public override bool hasBeenMet()
    {
        return CurrentExp >= Goal;
    }
}

public class UnlockableProgress
{
    public long Progress { get; set; }
    public DateTimeOffset Date { get; set; }
}