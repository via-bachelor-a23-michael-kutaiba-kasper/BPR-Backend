namespace UserManagementService.Domain.Models;

public class Unlockable
{
    public string Name { get; set; }
    public DateTimeOffset UnlockDate { get; set; }
    public string Description { get; set; }
    public long ExpReward { get; set; }
}