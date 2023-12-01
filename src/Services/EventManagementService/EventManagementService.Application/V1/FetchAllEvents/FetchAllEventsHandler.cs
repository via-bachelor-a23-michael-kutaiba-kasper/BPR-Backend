using EventManagementService.Application.V1.FetchAllEvents.Model;
using EventManagementService.Application.V1.FetchAllEvents.Repository;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.V1.FetchAllEvents;

public record FetchAllEventsRequest
(
    Filters Filters
) : IRequest<IReadOnlyCollection<Event>>;

public class FetchAllEventsHandler : IRequestHandler<FetchAllEventsRequest, IReadOnlyCollection<Event>>
{
    private readonly ISqlAllEvents _sqlAllEvents;
    private readonly ILogger<FetchAllEventsHandler> _logger;
    private readonly IUserRepository _userRepository;

    public FetchAllEventsHandler
    (
        ISqlAllEvents sqlAllEvents,
        ILogger<FetchAllEventsHandler> logger,
        IUserRepository userRepository
    )
    {
        _sqlAllEvents = sqlAllEvents;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        FetchAllEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        IDictionary<string, User> usersMap = new Dictionary<string, User>();
        var events = await AllEvents(request.Filters);
        foreach (var e in events)
        {
            var userIds = e.Attendees
                .Select(a => a.UserId)
                .Concat(new List<string> { e.Host.UserId });
            // NOTE: If performance becomes an issue, we can look into reducing the amount of network calls.
            var nonMappedUserIds = userIds.Where(id => !usersMap.ContainsKey(id));
            var nonMappedUsers = await _userRepository.GetUsersAsync(nonMappedUserIds.ToList());
            foreach (var user in nonMappedUsers)
            {
                if (usersMap.ContainsKey(user.UserId))
                {
                    continue;
                }

                usersMap[user.UserId] = user;
            }

            e.Host = usersMap[e.Host.UserId];
            e.Attendees = e.Attendees.Select(user => usersMap[user.UserId]);
        }

        return events;
    }

    private async Task<IReadOnlyCollection<Event>> AllEvents(Filters filters)
    {
        return await _sqlAllEvents.GetAllEvents(filters);
    }
}