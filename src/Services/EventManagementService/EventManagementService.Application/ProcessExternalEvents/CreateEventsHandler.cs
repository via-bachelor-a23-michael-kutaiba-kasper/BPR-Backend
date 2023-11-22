using EventManagementService.Application.ProcessExternalEvents.Exceptions;
using EventManagementService.Application.ProcessExternalEvents.Repository;
using EventManagementService.Application.ProcessExternalEvents.Util;
using EventManagementService.Domain.Models.Events;
using Google.Cloud.PubSub.V1;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.ProcessExternalEvents;

public record CreateEventsRequest
(
    TopicName TopicName,
    SubscriptionName SubscriptionName,
    IReadOnlyCollection<Event> Events
) : IRequest;

public class CreateEventsHandler : IRequestHandler<CreateEventsRequest>
{
    private readonly IGeoCoding _geoCoding;
    private readonly IPubSubExternalEvents _pubSubExternalEvents;
    private readonly ISqlExternalEvents _sqlExternalEvents;
    private readonly ILogger<CreateEventsHandler> _logger;

    public async Task Handle(CreateEventsRequest request, CancellationToken cancellationToken)
    {
        var newEvents = new List<Event>();

        try
        {
            _logger.LogInformation($"Creating new events ar: {DateTimeOffset.UtcNow}");
            var psEvents = await PubSubEvents(request, cancellationToken);
            var requestEvents = await ProcessRequestEvents(request);
            if (psEvents != null || psEvents.Any())
            {
                newEvents.AddRange(psEvents);
            }

            if (requestEvents != null || requestEvents.Any())
            {
                newEvents.AddRange(requestEvents);
            }

            await _sqlExternalEvents.BulkUpsertEvents(newEvents);
            _logger.LogInformation(
                $"{newEvents.Count} events have been successfully created at: {DateTimeOffset.UtcNow}");
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Something went wrong while trying to create new events at: {DateTimeOffset.UtcNow}");
            throw new CreateNewEventsException($"Cannot create new events at: {DateTimeOffset.UtcNow}", e);
        }
    }

    private async Task<IReadOnlyCollection<Event>> ProcessRequestEvents(CreateEventsRequest request)
    {
        var evs = new List<Event>();
        foreach (var e in request.Events)
        {
            evs.Add(new Event
            {
                Title = e.Title,
                Location = new Location
                {
                    Country = e.Location.Country,
                    StreetName = e.Location.StreetName,
                    StreetNumber = e.Location.StreetNumber,
                    HouseNumber = e.Location.HouseNumber,
                    PostalCode = e.Location.PostalCode,
                    City = e.Location.City,
                    SubPremise= e.Location.SubPremise,
                    GeoLocation = await FetchGeoLocation(e.Location)
                },
                Description = e.Description,
                Category = e.Category,
                Url = e.Url,
                Images = e.Images,
                Keywords = e.Keywords,
                AdultsOnly = e.AdultsOnly,
                EndDate = e.EndDate,
                CreatedDate = e.CreatedDate,
                HostId = e.HostId,
                IsPaid = e.IsPaid,
                IsPrivate = e.IsPrivate,
                StartDate = e.StartDate,
                LastUpdateDate = e.LastUpdateDate,
                MaxNumberOfAttendees = e.MaxNumberOfAttendees,
                AccessCode = UniqueEventAccessCodeGenerator.GenerateUniqueString(e.Title, e.CreatedDate)
            });
        }

        return evs;
    }

    private async Task<IReadOnlyCollection<Event>> PubSubEvents
    (
        CreateEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        var evs = new List<Event>();
        var psEvents =
            await _pubSubExternalEvents.FetchEvents(request.TopicName, request.SubscriptionName, cancellationToken);
        foreach (var e in psEvents)
        {
            evs.Add(new Event
            {
                Title = e.Title,
                Location = new Location
                {
                    Country = e.Location.Country,
                    StreetName = e.Location.StreetName,
                    StreetNumber = e.Location.StreetNumber,
                    HouseNumber = e.Location.HouseNumber,
                    PostalCode = e.Location.PostalCode,
                    City = e.Location.City,
                    SubPremise= e.Location.SubPremise,
                    GeoLocation = await FetchGeoLocation(e.Location)
                },
                Description = e.Description,
                Category = e.Category,
                Url = e.Url,
                Images = e.Images,
                Keywords = e.Keywords,
                AdultsOnly = e.AdultsOnly,
                EndDate = e.EndDate,
                CreatedDate = e.CreatedDate,
                HostId = e.HostId,
                IsPaid = e.IsPaid,
                IsPrivate = e.IsPrivate,
                StartDate = e.StartDate,
                LastUpdateDate = e.LastUpdateDate,
                MaxNumberOfAttendees = e.MaxNumberOfAttendees,
                AccessCode = UniqueEventAccessCodeGenerator.GenerateUniqueString(e.Title, e.CreatedDate)
            });
        }

        return evs;
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

        _logger.LogInformation($"Fetched GeoLocation ->: lat: {latLong.Lat}, lng: {latLong.Lng}");
        return latLong;
    }
}