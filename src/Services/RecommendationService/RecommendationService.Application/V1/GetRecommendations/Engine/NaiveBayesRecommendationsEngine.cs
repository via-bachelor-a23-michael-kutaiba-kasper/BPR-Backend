using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using RecommendationService.Application.V1.GetRecommendations.Util;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Engine;

// TODO: Implement this when we have more time in the future.
public class NaiveBayesRecommendationsEngine : IRecommendationsEngine
{
    public Recommendations Process(User user, IReadOnlyCollection<Event> completedEvents,
        IReadOnlyCollection<Review> reviews, InterestSurvey survey,
        IReadOnlyCollection<Event> futureEvents)
    {
        var relevantCompletedEvents =
            completedEvents
                .Where(e => e.Attendees != null)
                .Where(e => e.Attendees!.Any(u => u.UserId == user.UserId)).ToList();
        
        var futureEventsIndexed = IndexingUtil.IndexEvents(futureEvents);

        var context = new MLContext();
        var trainingData = context.Data.LoadFromEnumerable(ProcessData(relevantCompletedEvents, reviews));
        var pipeline = context.Transforms.Conversion
            .MapValueToKey(nameof(DataPoint.Label))
            .Append(context.MulticlassClassification.Trainers.NaiveBayes());
        var model = pipeline.Fit(trainingData);

        var recommendationCandidates = context.Data.LoadFromEnumerable(ProcessData(futureEvents, reviews));
        var recommendationCandidatesProcessed = model.Transform(recommendationCandidates);
        var recommendationPredictions = context.Data
            .CreateEnumerable<Prediction>(recommendationCandidatesProcessed, reuseRowObject: false)
            .ToList();

        var recommendations = recommendationPredictions
            .Where(prediction => prediction.PredictedLabel == 1)
            .Select(prediction => new Recommendation()
                {Event = futureEventsIndexed[(int) prediction.EventId], RelevanceScore = 1})
            .ToList();

        return new Recommendations
        {
            User = user,
            Result = recommendations,
            EventsProcessed = relevantCompletedEvents,
            ReviewsProcessed = reviews
        };
    }


    private IReadOnlyCollection<DataPoint> ProcessData(IReadOnlyCollection<Event> events,
        IReadOnlyCollection<Review> reviews)
    {
        List<DataPoint> dataPoints = new();
        IDictionary<int, Review> eventIdToReviewMapping = new Dictionary<int, Review>();

        foreach (var review in reviews)
        {
            if (eventIdToReviewMapping.ContainsKey(review.EventId))
            {
                continue;
            }

            eventIdToReviewMapping[review.EventId] = review;
        }

        foreach (var e in events)
        {
            var encodedCategoryFeatures = OneHotEncodeCategory(e);
            var encodedKeywordFeatures = OneHotEncodeKeywords(e);
            var startDateEncoded = EncodeDate(e.StartDate);
            var endDateEncoded = EncodeDate(e.EndDate);
            var hasReview = eventIdToReviewMapping.ContainsKey(e.Id);
            var reviewScore = hasReview ? new[] {eventIdToReviewMapping[e.Id].Rate} : new[] {3f};

            var features = encodedCategoryFeatures
                .Concat(encodedKeywordFeatures)
                .Concat(startDateEncoded)
                .Concat(endDateEncoded)
                .Concat(hasReview ? new[] {1f} : new[] {0f})
                .Concat(reviewScore);
            var target = reviewScore[0] >= 3 ? 1 : 0;

            var dataPoint = new DataPoint
            {
                EventId = (uint) e.Id,
                Label = (uint) target,
                Features = features.ToArray()
            };
            dataPoints.Add(dataPoint);
        }

        return dataPoints;
    }

    private float[] EncodeDate(DateTimeOffset date)
    {
        return new float[] {date.Year, date.Month, (int) date.DayOfWeek};
    }

    private float[] OneHotEncodeCategory(Event e)
    {
        var categories = Enum.GetValues<Category>();
        var encoded = new float[categories.Length];

        for (var i = 0; i < categories.Length; i++)
        {
            if (categories[i] == e.Category)
            {
                encoded[i] = 1;
            }
            else
            {
                encoded[i] = 0;
            }
        }

        return encoded;
    }

    private float[] OneHotEncodeKeywords(Event e)
    {
        var keywords = Enum.GetValues<Keyword>();
        var encoded = new float[keywords.Length];

        for (var i = 0; i < keywords.Length; i++)
        {
            if (e.Keywords.Contains(keywords[i]))
            {
                encoded[i] = 1;
            }
            else
            {
                encoded[i] = 0;
            }
        }

        return encoded;
    }

    private class DataPoint
    {
        public uint Label { get; set; }
        public uint EventId { get; set; }

        [VectorType(98)] // keywords (55) * categories (33) + start date(3) + end date(3) + has_review (1) + review_date(3) 
        public float[] Features { get; set; }
    }

    private class Prediction
    {
        public uint PredictedLabel { get; set; }
        public uint EventId { get; set; }
    }
}