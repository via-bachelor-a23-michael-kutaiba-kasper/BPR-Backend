namespace UserManagementService.Application.V1.ProcessUserAchievements.Model;

public class UserAchievementJoinTable
{
    public int achievement_id { get; set; }
    public string user_id { get; set; }
    public DateTimeOffset unlocked_date { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public long reward { get; set; }
    public string icon { get; set; }
}