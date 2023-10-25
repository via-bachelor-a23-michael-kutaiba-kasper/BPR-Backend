using EventManagementService.Application.FetchAllPublicEvents.Repository;
using EventManagementService.Domain.Models.Events;
using Google.Cloud.PubSub.V1;
using MediatR;
using Microsoft.Extensions.Logging;
using Location = EventManagementService.Domain.Models.Events.Location;

namespace EventManagementService.Application.FetchAllPublicEvents;

public record ScraperEventsRequest
(
    TopicName TopicName, SubscriptionName SubscriptionName
) : IRequest<IReadOnlyCollection<Event>>;

public class ScraperEventsHandler : IRequestHandler<ScraperEventsRequest, IReadOnlyCollection<Event>>
{
    private readonly IPubSubPublicEvents _pubSubPublicEvents;
    private readonly ISqlPublicEvents _sqlPublicEvents;
    private readonly IGeoCoding _geoCoding;
    private readonly ILogger<ScraperEventsHandler> _logger;

    public ScraperEventsHandler
    (
        IPubSubPublicEvents pubSubPublicEvents,
        ISqlPublicEvents sqlPublicEvents,
        IGeoCoding geoCoding,
        ILogger<ScraperEventsHandler> logger
    )
    {
        _pubSubPublicEvents = pubSubPublicEvents;
        _sqlPublicEvents = sqlPublicEvents;
        _geoCoding = geoCoding;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        ScraperEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        return await AllPublicEvents(request, cancellationToken);
    }

    private async Task<IReadOnlyCollection<Event>> AllPublicEvents
    (
        ScraperEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        var pubSubEvents = await PubSubEvents(request, cancellationToken);
        if (pubSubEvents.Count > 0)
            await UpsertNewPublicEvents(pubSubEvents);
        return await SqlEvents();
    }

    private async Task<IReadOnlyCollection<Event>> PubSubEvents
    (
        ScraperEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        return await _pubSubPublicEvents.FetchEvents(request.TopicName, request.SubscriptionName, cancellationToken);
    }

    private async Task<IReadOnlyCollection<Event>> SqlEvents()
    {
        return await _sqlPublicEvents.GetEvents();
    }

    private async Task UpsertNewPublicEvents(IReadOnlyCollection<Event> events)
    {
        var newEvents = new List<Event>();
        foreach (var e in events)
        {
            newEvents.Add(new Event
            {
                Title = e.Title,
                Description = e.Description,
                Url = e.Url,
                Location = new Location
                {
                    City = e.Location.City,
                    StreetNumber = e.Location.StreetNumber,
                    StreetName = e.Location.StreetName,
                    HouseNumber = e.Location.HouseNumber,
                    PostalCode = e.Location.PostalCode,
                    Country = e.Location.Country,
                    Floor = e.Location.Floor,
                    GeoLocation = await FetchGeoLocation(e.Location)
                }
            });
        }

        await _sqlPublicEvents.UpsertEvents(events);
    }

    private async Task<GeoLocation> FetchGeoLocation(Location location)
    {
        var address = @$"
                        {location.StreetName} 
                        {location.StreetNumber} 
                        {location.HouseNumber ?? ""} 
                        {location.PostalCode} 
                        {location.City} 
                        {location.Country}";
        var geo = await _geoCoding.FetchGeoLocationForAddress(address);

        return new GeoLocation
        {
            Lat = geo.Results.First().Geometry.Location.Lat,
            lng = geo.Results.First().Geometry.Location.Lng
        };
    }
}