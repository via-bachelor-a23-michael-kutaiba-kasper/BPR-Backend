using Microsoft.Extensions.Logging;
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
        var response = await _apiGateway.QueryAsync<List<Review>>(new ApiGatewayQuery
        {
            Query = EventsByUserQuery,
            Variables = new {userId}
        }, "reviewsByUser");

        return response.Result;
    }

    private string EventsByUserQuery => """
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