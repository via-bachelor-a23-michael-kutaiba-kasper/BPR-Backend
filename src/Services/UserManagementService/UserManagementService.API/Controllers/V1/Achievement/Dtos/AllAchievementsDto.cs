namespace UserManagementService.API.Controllers.V1.Achievement.Dtos;

public class AllAchievementsDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public long Reward { get; set; }
    public string Icon { get; set; }
    public int Requirement { get; set; }
}