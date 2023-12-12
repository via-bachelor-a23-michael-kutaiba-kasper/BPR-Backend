using Microsoft.Extensions.Logging;
using RecommendationService.Application.V1.GetRecommendations.Exceptions;
using RecommendationService.Domain.Events;
using RecommendationService.Infrastructure.ApiGateway;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface IReviewRepository
{
    Task<IReadOnlyCollection<Review>> GetReviewsByUserAsync(string userId);
}

public class ReviewRepository : IReviewRepository
{
    private readonly ILogger<ReviewRepository> _logger;
    private readonly IApiGateway _apiGateway;

    public ReviewRepository(ILogger<ReviewRepository> logger, IApiGateway apiGateway)
    {
        _logger = logger;
        _apiGateway = apiGateway;
    }

    public async Task<IReadOnlyCollection<Review>> GetReviewsByUserAsync(string userId)
    {
        try
        {
            var response = await _apiGateway.QueryAsync<List<Review>>(new ApiGatewayQuery
            {
                Query = EventsByUserQuery,
                Variables = new { userId }
            }, "reviewsByUser");

            return response.Result;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to fetch reviews from API Gateway");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw new FailedToFetchException("Reviews", "API Gateway", null, e);
        }
    }

    private static string EventsByUserQuery => """
                                               query ReviewsByUser($userId: String) {
                                                 reviewsByUser(userId: $userId) {
                                                   result {
                                                     id
                                                     rate
                                                     reviewerId
                                                     eventId
                                                     reviewDate
                                                   }
                                                   status {
                                                     code
                                                     message
                                                   }
                                                 }
                                               }
                                               """;
}