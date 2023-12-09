using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessUserAchievements.Checker;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;

namespace UserManagementService.Application.V1.ProcessUserAchievements;

public record ProcessUserAchievementsRequest(string UserId) : IRequest;

public class ProcessUserAchievementsHandle : IRequestHandler<ProcessUserAchievementsRequest>
{
    private readonly IEventRepository _eventRepository;
    private readonly ISqlAchievementRepository _sqlAchievementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProcessUserAchievementsHandle> _logger;

    public ProcessUserAchievementsHandle
    (
        IEventRepository eventRepository,
        ISqlAchievementRepository sqlAchievementRepository,
        IUserRepository userRepository,
        ILogger<ProcessUserAchievementsHandle> logger
    )
    {
        _eventRepository = eventRepository;
        _sqlAchievementRepository = sqlAchievementRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle
    (
        ProcessUserAchievementsRequest request,
        CancellationToken cancellationToken
    )
    {
        var userExists = await _userRepository.UserExists(request.UserId);
        if (!userExists)
        {
            throw new UserNotFoundException($"No user with id {request.UserId} have been found");
        }
    }

    private async Task Process
    (
        IReadOnlyCollection<Event>? events,
        IReadOnlyCollection<Achievement>? achievements,
        ProcessUserAchievementsRequest request
    )
    {
        if (events == null) return;
        var achievementMap = new Dictionary<Category, int>();

        var unlockedAchievements = await GetUserAchievements(request.UserId);

        var categoryCounts = events
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Count());
        var newAchievements = CollectAchievements(unlockedAchievements, categoryCounts);
        var userAch = new List<UserAchievementTable>();
        if (newAchievements != null)
        {
            userAch.AddRange(newAchievements.Select(achievement => new UserAchievementTable
            {
                user_id = request.UserId, unlocked_date = DateTimeOffset.UtcNow.ToUniversalTime(),
                achievement_id = (int)achievement
            }));
            await _sqlAchievementRepository.InsertUserAchievement(userAch);
        }
    }

    private static IReadOnlyCollection<UserAchievement>? CollectAchievements
    (
        IReadOnlyCollection<UserAchievementJoinTable> unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var achievements = new List<UserAchievement>();
        var canary = AchievementChecker.CheckMusicAndPerformingArts(unlockedAchievements, categoryCounts);
        var owl = AchievementChecker.CheckLearningAndDevelopment(unlockedAchievements, categoryCounts);
        var peacock = AchievementChecker.CheckCulturalAndArtistic(unlockedAchievements, categoryCounts);
        var bear = AchievementChecker.CheckCulinaryAndDrinks(unlockedAchievements, categoryCounts);
        var butterfly = AchievementChecker.CheckSocialAndCommunity(unlockedAchievements, categoryCounts);
        var cheetah = AchievementChecker.CheckHealthAndWellness(unlockedAchievements, categoryCounts);
        var monkey = AchievementChecker.CheckRecreationAndHobbies(unlockedAchievements, categoryCounts);
        if (canary != null)
        {
            achievements.AddRange(canary);
        }

        if (owl != null)
        {
            achievements.AddRange(owl);
        }

        if (peacock != null)
        {
            achievements.AddRange(peacock);
        }

        if (bear != null)
        {
            achievements.AddRange(bear);
        }

        if (butterfly != null)
        {
            achievements.AddRange(butterfly);
        }

        if (cheetah != null)
        {
            achievements.AddRange(cheetah);
        }

        if (monkey != null)
        {
            achievements.AddRange(monkey);
        }

        return achievements;
    }

    private async Task<IReadOnlyCollection<Event>> JoinedFinishedEvents(string userId)
    {
        return await _eventRepository.FetchJoinedFinishedEventsByUserId(userId);
    }

    private async Task<IReadOnlyCollection<UserAchievementJoinTable>?> GetUserAchievements(string userId)
    {
        return await _sqlAchievementRepository.GetUserAchievement(userId);
    }

    private async Task<IReadOnlyCollection<AchievementTable>> GetSystemAchievements()
    {
        return await _sqlAchievementRepository.GetAchievements();
    }
}