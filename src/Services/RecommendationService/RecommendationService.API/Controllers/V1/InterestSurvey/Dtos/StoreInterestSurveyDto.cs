namespace RecommendationService.API.Controllers.V1.InterestSurvey.Dtos;

public class StoreInterestSurveyDto
{
    public string UserId { get; set; }
    public IReadOnlyCollection<string> Keywords { get; set; }
    public IReadOnlyCollection<string> Categories { get; set; }
}