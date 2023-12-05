using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using RecommendationService.Domain;

namespace RecommendationService.Application.V1.GetRecommendations;

// NOTE: Keep this pure for easier testing, no side effects (network calls etc.)
public interface IRecommendationsEngine
{
    /// <summary>
    /// Process recommendations for future events based on completed events the user has attended in the past.
    /// </summary>
    /// <param name="user">User to process recommendations for</param>
    /// <param name="completedEvents">Completed events the user has attended</param>
    /// <param name="futureEvents">Events that are current (not started / in progress) that are valid recommendations</param>
    /// <returns></returns>
    Recommendations Process(User user, IReadOnlyCollection<Event> completedEvents,
        IReadOnlyCollection<Event> futureEvents);
}

public class RecommendationsEngine : IRecommendationsEngine
{
    public Recommendations Process(User user, IReadOnlyCollection<Event> completedEvents,
        IReadOnlyCollection<Event> futureEvents)
    {
        IDictionary<Category, int> categoryFrequencyMap = new Dictionary<Category, int>();
        IDictionary<Keyword, int> keywordFrequencyMap = new Dictionary<Keyword, int>();

        var relevantEvents = completedEvents
            .Where(e => e.Attendees != null)
            .Where(e => e.Attendees.Any(u => u.UserId == user.UserId));
        throw new NotImplementedException();
    }
}