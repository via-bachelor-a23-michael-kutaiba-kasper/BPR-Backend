namespace RecommendationService.API.Controllers.V1.Recommendation.Dtos;

public class ReadRecommendationsDto
{
    public ReadUserDto User { get; set; } = default!;
    public IReadOnlyCollection<ReadEventDto> EventsProcessed { get; set; } = new List<ReadEventDto>();
    public IReadOnlyCollection<ReadReviewDto> ReviewsProcessed { get; set; } = new List<ReadReviewDto>();
    public IReadOnlyCollection<ReadRecommendationDto> Result { get; set; } = new List<ReadRecommendationDto>();
}

public class ReadRecommendationDto
{
    public ReadEventDto Event { get; set; }
    public float RelevanceScore { get; set; }
}