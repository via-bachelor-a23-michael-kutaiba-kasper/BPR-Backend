using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Dto;

public class ProcessAchievementsDto
{
    public IReadOnlyCollection<Achievement>? Achievements { get; set; }
    public int CurrentProgress { get; set; }
}