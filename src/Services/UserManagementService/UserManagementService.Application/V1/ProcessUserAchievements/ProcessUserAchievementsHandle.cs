using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;
using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Domain.Util;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.Notifications;
using UserManagementService.Infrastructure.Notifications.Models;
using UserManagementService.Infrastructure.PubSub;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements;

public record ProcessUserAchievementsRequest(string UserId) : IRequest;

public class ProcessUserAchievementsHandle : IRequestHandler<ProcessUserAchievementsRequest>
{
    private readonly ISqlAchievementRepository _sqlAchievementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProcessUserAchievementsHandle> _logger;
    private readonly IReadOnlyCollection<CheckAchievementBaseStrategy> _strategies;
    private readonly INotifier _notifier;
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;

    public ProcessUserAchievementsHandle
    (
        ISqlAchievementRepository sqlAchievementRepository,
        IUserRepository userRepository,
        ILogger<ProcessUserAchievementsHandle> logger,
        IReadOnlyCollection<CheckAchievementBaseStrategy> strategies,
        INotifier notifier,
        IEventBus eventBus,
        IOptions<PubSub> pubsubConfig
    )
    {
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

            var events = await NewJoinedEvent(cancellationToken);
            foreach (var eEvent in events)
            {
                await ProcessAchievements(request.UserId, eEvent.Category);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Something went wrong while processing user achievement, {e.StackTrace}");
            throw new ProcessUserAchievementException("Unable to process user achievement", e);
        }
    }

    private async Task ProcessAchievements
    (
        string userId,
        Category category
    )
    {
        foreach (var strategy in _strategies)
        {
            await strategy.ProcessAchievement(userId, category);
        }
    }

    private async Task<IReadOnlyCollection<Event>> NewJoinedEvent(CancellationToken cancellationToken)
    {
        var ev = await _eventBus.PullAsync<Event>
        (
            _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewEvent].TopicId,
            _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewEvent].ProjectId,
            _pubsubConfig.Value.Topics[PubSubTopics.VibeVerseEventsNewEvent].SubscriptionNames[TopicSubs.UserManagementAchievements],
            10,
            cancellationToken
        );
        return ev.ToList();
    }

    private async Task<IReadOnlyCollection<UserAchievementJoinTable>?> GetUserAchievements(string userId)
    {
        return await _sqlAchievementRepository.GetUserAchievement(userId);
    }
}