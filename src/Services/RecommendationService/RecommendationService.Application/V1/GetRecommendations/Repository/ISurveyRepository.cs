using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface ISurveyRepository
{
    public Task<InterestSurvey> GetAsync(string userId);
}

public class SurveyRepository : ISurveyRepository
{
    // TODO: Implement this when the slice has been created.
    public async Task<InterestSurvey> GetAsync(string userId)
    {
        return new InterestSurvey()
        {
            Categories = new List<Category>() { Category.FoodAndDrink },
            Keywords = new List<Keyword>() { Keyword.FoodTasting },
            User = new User(){UserId = userId}
        };
    }
}