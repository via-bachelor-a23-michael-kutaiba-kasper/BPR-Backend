using EventManagementService.Application.V1.ReviewEvent.Exceptions;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using EventManagementService.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventManagementService.Application.V1.ReviewEvent.Repository;

public interface IPubSubReviewEvent
{
    Task PublishReviewedEvent(Review review);
}

public class PubSubReviewEvent : IPubSubReviewEvent
{
    private readonly IOptions<PubSub> _pubSubOptions;
    private readonly IEventBus _eventBus;
    private readonly ILogger<PubSubReviewEvent> _logger;

    public PubSubReviewEvent
    (
        IOptions<PubSub> pubSubOptions,
        IEventBus eventBus,
        ILogger<PubSubReviewEvent> logger
    )
    {
        _pubSubOptions = pubSubOptions;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task PublishReviewedEvent(Review review)
    {
        try
        {
            await _eventBus.PublishAsync
            (
                _pubSubOptions.Value.Topics[PubSubTopics.VibeVerseEventsNewReview].TopicId,
                _pubSubOptions.Value.Topics[PubSubTopics.VibeVerseEventsNewReview].ProjectId,
                review
            );
            _logger.LogInformation($"Review with id {review.Id} have been successfully published");
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to publish new review: {e.StackTrace}");
            throw new CannotPublishReviewException("Something went wrong while publishing new review", e);
        }
    }
}