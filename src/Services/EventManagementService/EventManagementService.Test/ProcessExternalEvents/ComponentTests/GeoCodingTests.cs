using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventManagementService.Application.ProcessExternalEvents.Repository;
using EventManagementService.Domain.Models.Google;
using EventManagementService.Infrastructure.Exceptions;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.ProcessExternalEvents.ComponentTests;

[TestFixture]
public class GeoCodingTests
{
    private Mock<ILogger<GeoCoding>> _loggerMock;
    private IGeoCoding _geoCoding;

    private readonly string _apiApi =
        Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

    [SetUp]
    public async Task Setup()
    {
        _loggerMock = new Mock<ILogger<GeoCoding>>();
    }

    [Test]
    public async Task GeoCode_LocationString_ThrowsNoExceptions()
    {
        // Arrange
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "GoogleGeoCodeResponse.json"
        );
        var file = await File.ReadAllTextAsync(path);
        var geoLocations = JsonSerializer.Deserialize<GoogleGeoLocation>(file, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        const string locationString = "Fussingvej 8, 8700 Horsens, Denmark";
        var factory = TestingUtil.CreateHttpClientFactoryMock(client =>
        {
            client.RegisterGetEndpoint(
                $"https://maps.googleapis.com/maps/api/geocode/json?address={locationString}&key={_apiApi}",
                HttpStatusCode.OK,
                file
            );
        });
        
         _geoCoding = new GeoCoding(_loggerMock.Object, factory.Object);
        
         // Act
         var act = async () => await _geoCoding.FetchGeoLocationForAddress(locationString);

         // Assert
         Assert.DoesNotThrowAsync(()=> act.Invoke());
    }
    
    [Test]
    public async Task GeoCode_UnsuccessfulResponse_ThrowsHttpException()
    {
        // Arrange
        const string locationString = "test";
        var factory = TestingUtil.CreateHttpClientFactoryMock(client =>
        {
            client.RegisterGetEndpoint(
                $"https://maps.googleapis.com/maps/api/geocode/json?address={locationString}&key={_apiApi}",
                HttpStatusCode.InternalServerError,
                ""
            );
        });
        
         _geoCoding = new GeoCoding(_loggerMock.Object, factory.Object);
        
         // Act
         var act = async () => await _geoCoding.FetchGeoLocationForAddress(locationString);

         // Assert
         Assert.ThrowsAsync<HttpException>(()=> act.Invoke());
    }
}