namespace RecommendationService.Domain.Events;

public class Review
{
    public int Id { get; set; } = default;
    public float Rate { get; set; } = default;
    public string ReviewerId { get; set; } = default!;
    public int EventId { get; set; } = default;
    public DateTimeOffset ReviewDate { get; set; } = default!;
}