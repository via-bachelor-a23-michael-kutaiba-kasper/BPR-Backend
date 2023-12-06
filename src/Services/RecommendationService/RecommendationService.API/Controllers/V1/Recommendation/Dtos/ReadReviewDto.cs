namespace RecommendationService.API.Controllers.V1.Recommendation.Dtos;

public class ReadReviewDto
{
    public int Id { get; set; }
    public float Rate { get; set; }
    public string ReviewerId { get; set; } = default!;
    public int EventId { get; set; }
    public DateTimeOffset ReviewDate { get; set; } = default!;
}