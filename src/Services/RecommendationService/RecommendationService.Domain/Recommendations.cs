using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;

namespace RecommendationService.Domain;

public class Recommendations
{
    public User User { get; set; } = default!;
    public IReadOnlyCollection<Event> EventsProcessed { get; set; } = new List<Event>();
    public IReadOnlyCollection<Recommendation> Result { get; set; } = new List<Recommendation>();
}