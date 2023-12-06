using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.V1.GetRecommendations.Engine;
using RecommendationService.Application.V1.GetRecommendations.Exceptions;
using RecommendationService.Application.V1.GetRecommendations.Repository;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations;

public record GetRecommendationsRequest(string UserId, int Limit = 10) : IRequest<Recommendations>;

public class GetRecommendationsHandler : IRequestHandler<GetRecommendationsRequest, Recommendations>
{
    private readonly ILogger<GetRecommendationsHandler> _logger;
    private readonly IRecommendationsEngine _engine;
    private readonly IEventsRepository _eventsRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;

    public GetRecommendationsHandler(
        ILogger<GetRecommendationsHandler> logger,
        IEventsRepository eventsRepository,
        IReviewRepository reviewRepository,
        ISurveyRepository surveyRepository,
        IUserRepository userRepository,
        IRecommendationsEngine? engine = null)
    {
        _logger = logger;
        _engine = engine ?? new FrequencyBasedRecommendationsEngine();
        _eventsRepository = eventsRepository;
        _reviewRepository = reviewRepository;
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
    }

    public async Task<Recommendations> Handle(GetRecommendationsRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.UserId);
        if (user is null)
        {
            throw new UserNotFoundException(request.UserId);
        }
            
        var reviews = await _reviewRepository.GetReviewsByUserAsync(request.UserId);
        var attendedEvents = await _eventsRepository.GetEventsWhereUserHasAttendedAsync(request.UserId);
        var futureEvents = await _eventsRepository.GetAllEvents(DateTimeOffset.UtcNow);
        var survey = await _surveyRepository.GetAsync(request.UserId);

        // _logger.LogInformation($"Processing recommendations for user {request.UserId} based on {reviews.Count} reviews, {attendedEvents.Count} completed events and {futureEvents.Count} future events");
        var recommendations = _engine.Process(user, attendedEvents, reviews, survey, futureEvents);
        recommendations.Result = recommendations.Result.Take(request.Limit).ToList();
        
        return recommendations;
    }
}