using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;

namespace RecommendationService.Domain.Events;

public class InterestSurvey
{
    public User User { get; set; } = default!;
    public IReadOnlyCollection<Keyword> Keywords { get; set; } = new List<Keyword>();
    public IReadOnlyCollection<Category> Categories { get; set; } = new List<Category>();
}