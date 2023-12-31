namespace UserManagementService.Domain.Models;

public class Progress
{
    public int Id { get; set; }
    public long TotalExp { get; set; }
    public Level Level { get; set; } = new();
    public int Stage { get; set; }
    public IReadOnlyCollection<ExpProgressEntry> ExpProgressHistory = new List<ExpProgressEntry>();
    public MonthlyGoal MonthlyGoal { get; set; } = new();
    public IReadOnlyCollection<Unlockable> Unlockables = new List<Unlockable>();
}

public class Level
{
    public int Value { get; set; } 
    public long MaxExp { get; set; }
    public long MinExp { get; set; }
    public string Name { get; set; } = "";
}

public class ExpProgressEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public long ExpGained { get; set; }
}