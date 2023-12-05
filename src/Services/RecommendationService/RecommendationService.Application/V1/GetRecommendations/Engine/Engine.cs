using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Engine;

// NOTE: Keep this pure for easier testing, no side effects (network calls etc.)
public interface IRecommendationsEngine
{
    /// <summary>
    /// Process recommendations for future events based on completed events the user has attended in the past.
    /// </summary>
    /// <param name="user">User to process recommendations for</param>
    /// <param name="completedEvents">Completed events the user has attended</param>
    /// <param name="reviews">Reviews that user has created</param>
    /// <param name="futureEvents">Events that are current (not started / in progress) that are valid recommendations</param>
    /// <returns></returns>
    Recommendations Process(User user, IReadOnlyCollection<Event> completedEvents, IReadOnlyCollection<Review> reviews, InterestSurvey survey,
        IReadOnlyCollection<Event> futureEvents);
}