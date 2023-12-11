namespace UserManagementServcie.Test.V1.FetchUserAchievements.Model;

public class UserAchievements
{
    public int AchievementId { get; set; }
    public string UserId { get; set; }
    public DateTimeOffset UnlocedDate { get; set; }
}