namespace UserManagementService.API.Controllers.V1.Achievement.Dtos;

public class AchievementDto
{
    public string Icon { get; set; }
    public string Name { get; set; }
    public DateTimeOffset UnlockDate { get; set; }
    public string Description { get; set; }
    public long ExpReward { get; set; }
    public int Progress { get; set; }
    public int Requirement { get; set; }
}