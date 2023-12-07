namespace RecommendationService.API.Controllers.V1.InterestSurvey.Dtos;



public class StoreInterestSurveyRequestDto
{
    public string UserId{ get; set; }
    public StoreInterestSurveyDto InterestSurvey{ get; set; }
}

public class StoreInterestSurveyDto
{
    public ReadUserDto User { get; set; }
    public IReadOnlyCollection<string> Keywords { get; set; }
    public IReadOnlyCollection<string> Categories { get; set; }
}