using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface ISurveyRepository
{
    public Task<InterestSurvey> GetAsync(string userId);
}