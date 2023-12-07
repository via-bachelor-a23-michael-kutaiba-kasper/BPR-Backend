namespace RecommendationService.API.Controllers.V1.Recommendation.Dtos;

public class ReadUserDto
{
    public string UserId { get; set; }
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTimeOffset? LastSeenOnline { get; set; }
    public DateTimeOffset CreationDate { get; set; }
}