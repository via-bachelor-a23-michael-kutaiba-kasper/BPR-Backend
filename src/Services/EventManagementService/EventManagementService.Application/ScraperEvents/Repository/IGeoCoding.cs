using EventManagementService.Domain.Models.Google;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.ScraperEvents.Repository;

public interface IGeoCoding
{
    Task<GeoLocation> FetchGeoLocationForAddress(string address);
}

public class GeoCoding : IGeoCoding
{
    private readonly ILogger<GeoCoding> _logger;

    public GeoCoding(ILogger<GeoCoding> logger)
    {
        _logger = logger;
    }

    public Task<GeoLocation> FetchGeoLocationForAddress(string address)
    {
        throw new NotImplementedException();
    }
}