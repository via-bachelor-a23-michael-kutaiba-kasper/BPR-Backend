using EventManagementService.Application.ProcessExternalEvents.Exceptions;
using EventManagementService.Application.ProcessExternalEvents.Repository;
using EventManagementService.Application.ProcessExternalEvents.Util;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.Util;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.ProcessExternalEvents;

public record ProcessExternalEventsRequest() : IRequest<IReadOnlyCollection<Event>>;

public class ProcessExternalEventsHandler : IRequestHandler<ProcessExternalEventsRequest, IReadOnlyCollection<Event>>
{
    private readonly IGeoCoding _geoCoding;
    private readonly IPubSubExternalEvents _pubSubExternalEvents;
    private readonly ISqlExternalEvents _sqlExternalEvents;
    private readonly ILogger<ProcessExternalEventsHandler> _logger;

    public ProcessExternalEventsHandler
    (
        IGeoCoding geoCoding,
        IPubSubExternalEvents pubSubExternalEvents,
        ISqlExternalEvents sqlExternalEvents,
        ILogger<ProcessExternalEventsHandler> logger
    )
    {
        _geoCoding = geoCoding;
        _pubSubExternalEvents = pubSubExternalEvents;
        _sqlExternalEvents = sqlExternalEvents;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> Handle
    (
        ProcessExternalEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            _logger.LogInformation($"Creating new events ar: {DateTimeOffset.UtcNow}");
            var pubSubEvents = await PubSubEvents(cancellationToken);
            await _sqlExternalEvents.BulkUpsertEvents(pubSubEvents);
            _logger.LogInformation(
                $"{pubSubEvents.Count} events have been successfully created at: {DateTimeOffset.UtcNow}");
            return pubSubEvents;
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Something went wrong while trying to create new events at: {DateTimeOffset.UtcNow}");
            throw new CreateNewEventsException($"Cannot create new events at: {DateTimeOffset.UtcNow}", e);
        }
    }

    private async Task<IReadOnlyCollection<Event>> PubSubEvents(CancellationToken cancellationToken)
    {
        var evs = new List<Event>();
        var psEvents = await _pubSubExternalEvents.FetchEvents(cancellationToken);
        foreach (var e in psEvents)
        {
            evs.Add(new Event
            {
                Title = e.Title,
                Location = e.Location,
                Description = e.Description,
                Category = e.Category,
                Url = e.Url,
                Images = e.Images,
                Keywords = e.Keywords,
                AdultsOnly = e.AdultsOnly,
                EndDate = new DateTimeOffset(),
                CreatedDate = DateTimeOffset.UtcNow,
                Host = new User
                {
                    UserId = e.Host.UserId,
                    CreationDate = e.Host.CreationDate,
                    DisplayName = e.Host.DisplayName,
                    PhotoUrl = e.Host.PhotoUrl,
                    LastSeenOnline = e.Host.LastSeenOnline
                },
                IsPaid = e.IsPaid,
                IsPrivate = e.IsPrivate,
                StartDate = new DateTimeOffset(),
                LastUpdateDate = new DateTimeOffset(),
                MaxNumberOfAttendees = e.MaxNumberOfAttendees,
                AccessCode = UniqueEventAccessCodeGenerator.GenerateUniqueString(e.Title, e.CreatedDate),
                City = e.City,
                GeoLocation = await FetchGeoLocation(e.Location)
            });
        }

        return evs;
    }

    private async Task<GeoLocation> FetchGeoLocation(string location)
    {
        var geo = await _geoCoding.FetchGeoLocationForAddress(location);

        var latLong = new GeoLocation
        {
            Lat = geo.Results.First().Geometry.Location.Lat,
            Lng = geo.Results.First().Geometry.Location.Lng
        };

        _logger.LogInformation($"Fetched GeoLocation ->: lat: {latLong.Lat}, lng: {latLong.Lng}");
        return latLong;
    }
}