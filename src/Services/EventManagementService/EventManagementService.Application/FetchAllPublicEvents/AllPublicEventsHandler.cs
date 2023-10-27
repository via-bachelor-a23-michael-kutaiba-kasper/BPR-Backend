using EventManagementService.Application.FetchAllPublicEvents.Repository;
using EventManagementService.Domain.Models.Events;
using Google.Cloud.PubSub.V1;
using MediatR;
using Microsoft.Extensions.Logging;
using Location = EventManagementService.Domain.Models.Events.Location;

namespace EventManagementService.Application.FetchAllPublicEvents;

public record AllPublicEventsRequest
(
    TopicName TopicName, SubscriptionName SubscriptionName
) : IRequest<IReadOnlyCollection<Event>>;

public class AllPublicEventsHandler : IRequestHandler<AllPublicEventsRequest, IReadOnlyCollection<Event>>
{
    private readonly IPubSubPublicEvents _pubSubPublicEvents;
    private readonly ISqlPublicEvents _sqlPublicEvents;
    private readonly IGeoCoding _geoCoding;
    private readonly ILogger<AllPublicEventsHandler> _logger;

    public AllPublicEventsHandler
    (
        IPubSubPublicEvents pubSubPublicEvents,
        ISqlPublicEvents sqlPublicEvents,
        IGeoCoding geoCoding,
        ILogger<AllPublicEventsHandler> logger
    )
    {
        _pubSubPublicEvents = pubSubPublicEvents;
        _sqlPublicEvents = sqlPublicEvents;
        _geoCoding = geoCoding;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        AllPublicEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        return await AllPublicEvents(request, cancellationToken);
    }

    private async Task<IReadOnlyCollection<Event>> AllPublicEvents
    (
        AllPublicEventsRequest request,
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
        AllPublicEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        return await _pubSubPublicEvents.FetchEvents(request.TopicName, request.SubscriptionName, cancellationToken);
    }

    private async Task<IReadOnlyCollection<Event>> SqlEvents()
    {
        return await _sqlPublicEvents.GetAllEvents();
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

        await _sqlPublicEvents.UpsertEvents(newEvents);
    }

    private async Task<GeoLocation> FetchGeoLocation(Location location)
    {
        var address =
            $"{location.StreetName} {location.StreetNumber} {location.HouseNumber ?? ""} {location.PostalCode} {location.City} {location.Country}";
        var geo = await _geoCoding.FetchGeoLocationForAddress(address);

        var latLong = new GeoLocation
        {
            Lat = geo.Results.First().Geometry.Location.Lat,
            Lng = geo.Results.First().Geometry.Location.Lng
        };

        _logger.LogDebug($"Fetched GeoLocation ->: {latLong}");
        return latLong;
    }
}