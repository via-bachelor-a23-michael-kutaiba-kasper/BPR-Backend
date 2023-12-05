using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface IReviewRepository
{
    Task<IReadOnlyCollection<Review>> GetReviewsByUserAsync(string userId);
}