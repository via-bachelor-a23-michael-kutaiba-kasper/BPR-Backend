using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Microsoft.ML;
using Microsoft.ML.Data;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Engine;

public class NaiveBayesRecommendationsEngine: IRecommendationsEngine
{
    public Recommendations Process(User user, IReadOnlyCollection<Event> completedEvents, IReadOnlyCollection<Review> reviews,
        IReadOnlyCollection<Event> futureEvents)
    {
        var context = new MLContext();
        
        throw new NotImplementedException();
    }

    private class DataPoint
    {
        public uint Label { get; set; }
        [VectorType(85)]
        public float[] Features { get; set; }
    }
}

