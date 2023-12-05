using EventManagementService.Domain.Models.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Util;

public static class EventUtil
{
    public static IDictionary<int, Event> IndexEvents(IReadOnlyCollection<Event> events)
    {
        IDictionary<int, Event> eventsMap = new Dictionary<int, Event>();
        foreach (var e in events)
        {
            if (eventsMap.ContainsKey(e.Id))
            {
                continue;
            }

            eventsMap.Add(e.Id, e);
        }

        return eventsMap;
    }
}