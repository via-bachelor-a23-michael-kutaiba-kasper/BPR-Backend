namespace UserManagementService.Application.V1.ProcessUserAchievements.Model;

public class UnlockableAchievementProgressTable
{
    public int id { get; set; }
    public int achievement_id { get; set; }
    public string user_id { get; set; }
    public long progress { get; set; }
    public DateTimeOffset date { get; set; }
}