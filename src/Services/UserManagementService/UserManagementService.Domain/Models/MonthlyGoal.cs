namespace UserManagementService.Domain.Models;

public class MonthlyGoal
{
    public string Description { get; set; } = "";
    public DateTimeOffset StartTime { get; set; }
    public UnlockCriteria UnlockCriteria { get; set; }
}