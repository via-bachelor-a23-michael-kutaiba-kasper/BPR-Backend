using MediatR;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Exceptions;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Repositories;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Validation;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult;

public record StoreInterestSurveyRequest(string userId, InterestSurvey survey) : IRequest<InterestSurvey>;

public class StoreInterestSurveyResultHandler : IRequestHandler<StoreInterestSurveyRequest, InterestSurvey>
{
    private readonly ILogger<StoreInterestSurveyResultHandler> _logger;
    private readonly IInterestSurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;

    public StoreInterestSurveyResultHandler(
        ILogger<StoreInterestSurveyResultHandler> logger,
        IInterestSurveyRepository surveyRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
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
        
        validator.validate(request.survey);

        _logger.LogInformation($"Storing interest survey for user {request.userId}");
        var storedSurvey = await _surveyRepository.StoreInterestSurvey(request.userId, request.survey);
        storedSurvey.User = existingUser;
        
        return storedSurvey;
    }
}