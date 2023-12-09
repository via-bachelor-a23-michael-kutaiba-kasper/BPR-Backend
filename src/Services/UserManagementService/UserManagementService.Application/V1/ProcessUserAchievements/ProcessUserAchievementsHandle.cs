using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;
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
    private readonly IReadOnlyCollection<CheckAchievementBaseStrategy> _strategies;

    public ProcessUserAchievementsHandle
    (
        IEventRepository eventRepository,
        ISqlAchievementRepository sqlAchievementRepository,
        IUserRepository userRepository,
        ILogger<ProcessUserAchievementsHandle> logger,
        IReadOnlyCollection<CheckAchievementBaseStrategy> strategies
    )
    {
        _eventRepository = eventRepository;
        _sqlAchievementRepository = sqlAchievementRepository;
        _userRepository = userRepository;
        _logger = logger;
        _strategies = strategies;
    }

    public async Task Handle
    (
        ProcessUserAchievementsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var userExists = await _userRepository.UserExists(request.UserId);
            if (!userExists)
            {
                throw new UserNotFoundException($"No user with id {request.UserId} have been found");
            }

            var events = await JoinedFinishedEvents(request.UserId);

            await Process(events, request);
        }
        catch (Exception e)
        {
            _logger.LogError($"Something went wrong while processing user achievement, {e.StackTrace}");
            throw new ProcessUserAchievementException("Unable to process user achievement", e);
        }

        ;
    }

    private async Task Process
    (
        IReadOnlyCollection<Event>? events,
        ProcessUserAchievementsRequest request
    )
    {
        if (events == null) return;
        var achievementMap = new Dictionary<Category, int>();

        var unlockedAchievements = await GetUserAchievements(request.UserId) ?? new List<UserAchievementJoinTable>();

        var categoryCounts = events
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Count());
        var newAchievements = CollectAchievements(unlockedAchievements, categoryCounts) ?? new List<UserAchievement>();

        var userAch = new List<UserAchievementTable>();

        userAch.AddRange(newAchievements.Select(achievement => new UserAchievementTable
        {
            user_id = request.UserId, unlocked_date = DateTimeOffset.UtcNow.ToUniversalTime(),
            achievement_id = (int)achievement
        }));
        await _sqlAchievementRepository.InsertUserAchievement(userAch);
    }

    private IReadOnlyCollection<UserAchievement>? CollectAchievements
    (
        IReadOnlyCollection<UserAchievementJoinTable> unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var achievements = new List<UserAchievement>();
        foreach (var strategy in _strategies)
        {
            achievements.AddRange(strategy.CheckAchievement(unlockedAchievements, categoryCounts));
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