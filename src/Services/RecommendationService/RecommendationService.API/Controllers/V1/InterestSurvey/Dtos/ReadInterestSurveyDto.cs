namespace RecommendationService.API.Controllers.V1.InterestSurvey.Dtos;

public class ReadInterestSurveyDto
{
    public ReadUserDto User { get; set; }
    public IReadOnlyCollection<string> Keywords { get; set; }
    public IReadOnlyCollection<string> Categories { get; set; }
}