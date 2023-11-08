using EventManagementService.Domain.Models.Events;
using Google.Cloud.PubSub.V1;
using MediatR;

namespace EventManagementService.Application.CreateEvents;

public record CreateEventsRequest
(
    TopicName TopicName, SubscriptionName SubscriptionName
) : IRequest<IReadOnlyCollection<Event>>;

public class CreateEventsHandler
{
    /*
     

    private async Task<IReadOnlyCollection<Event>> PubSubEvents
    (
        AllPublicEventsRequest request,
        CancellationToken cancellationToken
    )
    {
        return await _pubSubPublicEvents.FetchEvents(request.TopicName, request.SubscriptionName, cancellationToken);
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
     */
}