namespace UserManagementService.Application.V1.ProcessUserAchievements.Model;

public class UserAchievementTable
{
    public int achievement_id { get; set; }
    public string user_id { get; set; }
    public DateTimeOffset unlocked_date { get; set; }
}