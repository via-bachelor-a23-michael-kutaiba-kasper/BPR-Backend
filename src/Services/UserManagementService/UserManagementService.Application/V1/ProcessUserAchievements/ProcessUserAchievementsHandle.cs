using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;
using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.Notifications;
using UserManagementService.Infrastructure.Notifications.Models;
using UserManagementService.Infrastructure.PubSub;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements;

public record ProcessUserAchievementsRequest(string UserId) : IRequest;

public class ProcessUserAchievementsHandle : IRequestHandler<ProcessUserAchievementsRequest>
{
    private readonly IEventRepository _eventRepository;
    private readonly ISqlAchievementRepository _sqlAchievementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProcessUserAchievementsHandle> _logger;
    private readonly IReadOnlyCollection<CheckAchievementBaseStrategy> _strategies;
    private readonly INotifier _notifier;
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;

    public ProcessUserAchievementsHandle
    (
        IEventRepository eventRepository,
        ISqlAchievementRepository sqlAchievementRepository,
        IUserRepository userRepository,
        ILogger<ProcessUserAchievementsHandle> logger,
        IReadOnlyCollection<CheckAchievementBaseStrategy> strategies,
        INotifier notifier,
        IEventBus eventBus,
        IOptions<PubSub> pubsubConfig
    )
    {
        _eventRepository = eventRepository;
        _sqlAchievementRepository = sqlAchievementRepository;
        _userRepository = userRepository;
        _logger = logger;
        _strategies = strategies;
        _notifier = notifier;
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
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

            var events = await NewJoinedEvent();

            await Process(events, request);
        }
        catch (Exception e)
        {
            _logger.LogError($"Something went wrong while processing user achievement, {e.StackTrace}");
            throw new ProcessUserAchievementException("Unable to process user achievement", e);
        }
    }

    private async Task Process
    (
        IReadOnlyCollection<Event>? events,
        ProcessUserAchievementsRequest request
    )
    {
        if (events == null) return;

        var unlockedAchievements = await GetUserAchievements(request.UserId) ?? new List<UserAchievementJoinTable>();

        var categoryCounts = events
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Count());
        var newAchievements = CollectAchievements(unlockedAchievements, categoryCounts);
        var inProgressAchievements = new List<UserAchievement>();
        var newUnlockedAchievements = new List<UserAchievement>();

        foreach (var achievement in newAchievements)
        {
            if (achievement.Key == AchievementsTypes.AlreadyUnlocked) continue;
            if (achievement.Key == AchievementsTypes.InProgress)
            {
                inProgressAchievements = achievement.Value.ToList();
            }

            if (achievement.Key == AchievementsTypes.Unlocked)
            {
                newUnlockedAchievements = achievement.Value.ToList();
            }
        }

        var prog = new List<UnlockableAchievementProgressTable>();

        foreach (var ia in inProgressAchievements)
        {
            var curentProgress =
                await _sqlAchievementRepository.GetCountOFCurrentAchievementProgress(request.UserId, (int)ia);
            prog.Add(new UnlockableAchievementProgressTable
            {
                achievement_id = (int)ia,
                progress = curentProgress + inProgressAchievements.Count,
                date = DateTimeOffset.UtcNow.ToUniversalTime(),
                user_id = request.UserId
            });
        }

        var nw = new List<UserAchievementTable>();
        foreach (var na in newUnlockedAchievements)
        {
            nw.Add(new UserAchievementTable
            {
                user_id = request.UserId,
                unlocked_date = DateTimeOffset.UtcNow.ToUniversalTime(),
                achievement_id = (int)na
            });
        }

        await _sqlAchievementRepository.UpsertAchievementProgress(prog);
        await _sqlAchievementRepository.InsertUserAchievement(nw);

        try
        {
            foreach (var ac in nw)
            {
                var hostNotificationToken =
                    await _userRepository.GetNotificationTokenByUserIdAsync(request.UserId);
                await _notifier.SendNotificationAsync(new UserNotification
                {
                    Title = "You got a new achievements!!",
                    Body = $"Congratulations you gained: {((UserAchievement)ac.achievement_id).GetDescription()}",
                    Token = hostNotificationToken
                });
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"{e.StackTrace}");
        }
    }

    private Dictionary<string, IReadOnlyCollection<UserAchievement>> CollectAchievements
    (
        IReadOnlyCollection<UserAchievementJoinTable> unlockedAchievements,
        Dictionary<Category, int> categoryCounts
    )
    {
        var results = new Dictionary<string, IReadOnlyCollection<UserAchievement>>();
        var achievements = new List<UserAchievement>();
        foreach (var strategy in _strategies)
        {
            var stratResult = strategy.CheckAchievement(unlockedAchievements, categoryCounts);
            results.Concat(stratResult).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        return results;
    }

    private async Task<IReadOnlyCollection<Event>> NewJoinedEvent()
    {
        var ev = await _eventBus.PullAsync<Event>
        (
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].TopicId,
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].ProjectId,
            _pubsubConfig.Value.Topics[PubSubTopics.NewSurvey].SubscriptionNames[TopicSubs.UserManagementAchievements],
            10,
            new CancellationToken()
        );
        return ev.ToList();
    }

    private async Task<IReadOnlyCollection<UserAchievementJoinTable>?> GetUserAchievements(string userId)
    {
        return await _sqlAchievementRepository.GetUserAchievement(userId);
    }
}