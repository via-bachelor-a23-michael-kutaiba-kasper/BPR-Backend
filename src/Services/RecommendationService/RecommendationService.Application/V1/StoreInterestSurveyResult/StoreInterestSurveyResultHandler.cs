using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Exceptions;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Repositories;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Validation;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;
using RecommendationService.Infrastructure;
using RecommendationService.Infrastructure.AppSettings;
using RecommendationService.Infrastructure.EventBus;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult;

public record StoreInterestSurveyRequest(string userId, InterestSurvey survey) : IRequest<InterestSurvey>;

public class StoreInterestSurveyResultHandler : IRequestHandler<StoreInterestSurveyRequest, InterestSurvey>
{
    private readonly ILogger<StoreInterestSurveyResultHandler> _logger;
    private readonly IInterestSurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;
    private readonly IOptions<PubSub> _pubsubConfig;

    public StoreInterestSurveyResultHandler
    (
        ILogger<StoreInterestSurveyResultHandler> logger,
        IInterestSurveyRepository surveyRepository,
        IUserRepository userRepository,
        IEventBus eventBus,
        IOptions<PubSub> pubsubConfig
    )
    {
        _logger = logger;
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
        _eventBus = eventBus;
        _pubsubConfig = pubsubConfig;
    }

    public async Task<InterestSurvey> Handle(StoreInterestSurveyRequest request, CancellationToken cancellationToken)
    {
        var validator = new InterestSurveyValidator();
        _logger.LogInformation($"Received {nameof(StoreInterestSurveyRequest)}");

        var existingUser = await _userRepository.GetByIdAsync(request.userId);
        if (existingUser is null)
        {
            throw new UserNotFoundException(request.userId);
        }

        var existingInterestSurvey = await _surveyRepository.GetInterestSurvey(request.userId);
        if (existingInterestSurvey is not null)
        {
            throw new InterestSurveyAlreadyCompletedException(request.userId);
        }

        validator.Validate(request.survey);

        _logger.LogInformation($"Storing interest survey for user {request.userId}");
        var storedSurvey = await _surveyRepository.StoreInterestSurvey(request.userId, request.survey);
        storedSurvey.User = existingUser;

        try
        {
            var topic = _pubsubConfig.Value.Topics[PubSubTopics.NewInterestSurvey];
            await _eventBus.PublishAsync(topic.TopicId, topic.ProjectId, existingUser.UserId);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to publish new interest survey message to Event Bus");
        }

        return storedSurvey;
    }
}