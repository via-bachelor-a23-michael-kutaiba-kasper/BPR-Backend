using MediatR;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.V1.GetInterestSurvey.Exceptions;
using RecommendationService.Application.V1.GetInterestSurvey.Repositories;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetInterestSurvey;

public record GetInterestSurveyRequest(string userId) : IRequest<InterestSurvey?>;

public class GetInterestSurveyHandler : IRequestHandler<GetInterestSurveyRequest, InterestSurvey?>
{
    private readonly ILogger<GetInterestSurveyHandler> _logger;
    private readonly IInterestSurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;

    public GetInterestSurveyHandler(ILogger<GetInterestSurveyHandler> logger,
        IInterestSurveyRepository surveyRepository, IUserRepository userRepository)
    {
        _logger = logger;
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
    }

    public async Task<InterestSurvey?> Handle(GetInterestSurveyRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByIdAsync(request.userId);
        if (existingUser is null)
        {
            throw new UserNotFoundException(request.userId);
        }

        _logger.LogInformation($"Retrieving interest survey for {request.userId}");
        var interestSurvey = await _surveyRepository.GetInterestSurvey(request.userId);

        _logger.LogInformation(interestSurvey is null
            ? $"User {request.userId} has not yet filled out interest survey, returning null"
            : $"Retrieved interest survey for user {request.userId} successfully");

        return interestSurvey;
    }
}