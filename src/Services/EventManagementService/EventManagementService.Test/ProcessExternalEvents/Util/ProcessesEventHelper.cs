using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventManagementService.Application.ProcessExternalEvents.Repository;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.ProcessExternalEvents.Util;

public class ProcessesEventHelper
{
    private readonly TestDataContext _context = new();
    private readonly ConnectionStringManager _connectionStringManager = new();

    [SetUp]
    public async Task Setup()
    {
        _context.ConnectionString = _connectionStringManager.GetConnectionString();
        await _context.Clean();
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.ConnectionString = _connectionStringManager.GetConnectionString();
        await _context.Clean();
    }

    [Test]
    public async Task BulkInsert_ExternalEvents_ThrowNoExceptions()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SqlExternalEvents>>();
        var loggerMock2 = new Mock<ILogger<GeoCoding>>();
        var facmock = new Mock<IHttpClientFactory>();
       // var repo = new SqlExternalEvents(loggerMock.Object, _connectionStringManager);
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "ExternalEventsResponse.json"
        );
        // remember to set google api key
        facmock.Setup(x => x.CreateClient("HTTP_CLIENT")).Returns(new HttpClient());
        var geo = new GeoCoding(loggerMock2.Object, facmock.Object);
        
        
        
        var file = await File.ReadAllTextAsync(path);
        var events = JsonSerializer.Deserialize<List<Event>>(file, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var evs = await Evs(geo, events!);

        var ser = JsonSerializer.Serialize(evs, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var path2 = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "File.json"
        );
        
        //await File.WriteAllTextAsync(path2, ser);
        Console.Write(ser);
        
        
        /*// Act
        var act = async () => await repo.BulkUpsertEvents(events!);

        // Assert
        Assert.DoesNotThrowAsync(()=> act.Invoke());*/
    }
    
    private async Task<GeoLocation> FetchGeoLocation(IGeoCoding _geoCoding, string location)
    {
        var geo = await _geoCoding.FetchGeoLocationForAddress(location);

        var latLong = new GeoLocation
        {
            Lat = geo.Results.First().Geometry.Location.Lat,
            Lng = geo.Results.First().Geometry.Location.Lng
        };
        
        return latLong;
    }
    
    private async Task<IReadOnlyCollection<Event>> Evs
    (
        IGeoCoding geoCoding,
        List<Event> events
    )
    {
        var evs = new List<Event>();
        var psEvents =
            events;
        
        for (int i = 0; i < psEvents.Count; i++)
        {
            evs.Add(new Event
            {
                Title = psEvents[i].Title,
                Location = psEvents[i].Location,
                Description = psEvents[i].Description,
                Category = psEvents[i].Category,
                Url = psEvents[i].Url,
                Images = psEvents[i].Images,
                Keywords = psEvents[i].Keywords,
                AdultsOnly = psEvents[i].AdultsOnly,
                EndDate = new DateTimeOffset(2023, 12, 28, 12, i, 0, TimeSpan.Zero),
                CreatedDate = DateTimeOffset.UtcNow,
                HostId = psEvents[i].HostId,
                IsPaid = psEvents[i].IsPaid,
                IsPrivate = psEvents[i].IsPrivate,
                StartDate = new DateTimeOffset(2023, 11, 28, 12, i, 0, TimeSpan.Zero),
                LastUpdateDate = new DateTimeOffset(),
                MaxNumberOfAttendees = psEvents[i].MaxNumberOfAttendees,
                AccessCode = GenerateUniqueString(psEvents[i].Title, psEvents[i].CreatedDate),
                GeoLocation = await FetchGeoLocation(geoCoding, psEvents[i].Location),
                City = psEvents[i].City
            });
        }

        return evs;
    }
    
    private static string GenerateUniqueString(string title, DateTimeOffset creationDate)
    {
        var combinedInfo = $"{title}_{creationDate.ToString("yyyyMMddHHmmssfffzzz")}";

        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInfo));

            // Convert the hashed bytes to a string
            var stringBuilder = new StringBuilder();
            foreach (var t in hashBytes)
            {
                stringBuilder.Append(t.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}