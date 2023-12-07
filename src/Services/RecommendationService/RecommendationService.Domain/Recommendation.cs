using EventManagementService.Domain.Models.Events;

namespace RecommendationService.Domain;

public class Recommendation
{
    public Event Event { get; set; }
    public float RelevanceScore { get; set; }
}