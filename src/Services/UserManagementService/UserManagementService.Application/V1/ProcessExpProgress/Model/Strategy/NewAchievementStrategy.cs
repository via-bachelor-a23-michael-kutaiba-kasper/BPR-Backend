using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

public class NewAchievementStrategy: IExpStrategy
{
    private readonly IAchievementsRepository _achievementsRepository;

    public NewAchievementStrategy(IAchievementsRepository achievementsRepository)
    {
        _achievementsRepository = achievementsRepository;
    }

    public async Task Register(ExperienceGainedLedger ledger, ILogger logger)
    {
        logger.LogInformation("Processing newly unlocked achievements experience gains");
        var newAchievements = (await _achievementsRepository.GetNewlyUnlockedAchievements()).ToList();
        foreach (var unlockedAchievement in newAchievements)
        {
            ledger.RegisterExpGeneratingEvent(unlockedAchievement.UserId, e => new NewAchievementEvent(e, unlockedAchievement.Reward));
        }
        logger.LogInformation($"Processed {newAchievements.Count} newly unlocked achievement(s)");
    }
}