using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Engine;

public class SimpleRecommendationsEngine : IRecommendationsEngine
{
    public Recommendations Process(User user, IReadOnlyCollection<Event> completedEvents,
        IReadOnlyCollection<Review> reviews,
        IReadOnlyCollection<Event> futureEvents)
    {
        var relevantEvents = completedEvents
            .Where(e => e.Attendees != null)
            .Where(e => e.Attendees!.Any(u => u.UserId == user.UserId)).ToList();
        var relevantReviews = reviews
            .Where(review => review.ReviewerId == user.UserId)
            .ToList();

        var weights = CalculateWeights(GetFrequencyMaps(relevantEvents), relevantReviews);

        // tuple where (score, event)
        var scoredEvents = futureEvents.Select(e => (ScoreEvent(e, weights), e));

        var rankedRecommendations = scoredEvents
            .OrderByDescending(pair => pair.Item1) // Highest score first
            .Select(pair => new Recommendation {Event = pair.e, RelevanceScore = pair.Item1})
            .ToList();

        return new Recommendations
        {
            User = user,
            EventsProcessed = relevantEvents,
            ReviewsProcessed = reviews,
            Result = rankedRecommendations
        };
    }

    private float ScoreEvent(Event e, WeightMaps weights)
    {
        float baseline = 1.0f;
        float categoryFactor = 1.0f;

        categoryFactor = weights.CategoryWeights.TryGetValue(e.Category, out float categoryWeight)
            ? categoryWeight
            : 1.0f;

        var keywordFactors = e.Keywords
            .Where(weights.KeywordWeights.ContainsKey)
            .Select(kw => weights.KeywordWeights[kw])
            .ToList();

        float finalScore = baseline * categoryFactor;
        keywordFactors.ForEach(factor => finalScore = finalScore * factor);

        return finalScore;
    }

    private WeightMaps CalculateWeights(FrequencyMaps frequencyMaps, IReadOnlyCollection<Review> reviews)
    {
        throw new NotImplementedException();
    }

    private FrequencyMaps GetFrequencyMaps(IReadOnlyCollection<Event> events)
    {
        IDictionary<Category, int> categoryFrequencyMap = new Dictionary<Category, int>();
        IDictionary<Keyword, int> keywordFrequencyMap = new Dictionary<Keyword, int>();
        foreach (var @event in events)
        {
            Category category = @event.Category;
            if (!categoryFrequencyMap.ContainsKey(category))
            {
                categoryFrequencyMap[category] = 1;
            }
            else
            {
                categoryFrequencyMap[category] += 1;
            }

            @event.Keywords.ToList().ForEach(kw =>
            {
                if (!keywordFrequencyMap.ContainsKey(kw))
                {
                    keywordFrequencyMap[kw] = 1;
                }
                else
                {
                    keywordFrequencyMap[kw] += 1;
                }
            });
        }

        return new FrequencyMaps(categoryFrequencyMap, keywordFrequencyMap);
    }
}