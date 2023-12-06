using System.Net;
using RecommendationService.API.Controllers.V1.Recommendation.Dtos;
using RecommendationService.Infrastructure;
using RecommendationService.Infrastructure.ApiGateway;
using RecommendationService.Infrastructure.AppSettings;
using RecommendationService.Test.Shared;

namespace RecommendationService.Test.ApiGateway;

public class ApiGatewayComponentTests
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
    public async Task QueryAsync_QueryEvents_ParsesResponseCorrectly()
    {
        // Arrange
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ApiGateway", "Fakes",
            "ApiGatewayEventsQueryResponse.json");

        var eventsQueryResponse= await File.ReadAllTextAsync(path);
        var httpClientFactory = TestingUtil.CreateHttpClientFactoryMock(client =>
        {
            client.RegisterGetEndpoint($"http://gateway.test", HttpStatusCode.OK, eventsQueryResponse);
        });
        
        IApiGateway apiGateway =
            new Infrastructure.ApiGateway.ApiGateway(new Gateway { Url = "http://gateway.test" }, httpClientFactory.Object.CreateClient("HTTP_CLIENT"));
        
        // Act 
        var response = await apiGateway.QueryAsync<IEnumerable<ReadEventDto>>(new ApiGatewayQuery
        {
            Query = EventsQuery(),
            Variables = new {userId = "test"}
        }, "events");
        
        // Assert
        Assert.That(response.Status.Code, Is.EqualTo(200)); 
        Assert.That(response.Result.Count(), Is.GreaterThan(1)); 
    }
    private string EventsQuery() => """
      query Events($from: String) {
        events(from: $from) {
          result {
            id
            title
            startDate
            endDate
            createdDate
            lastUpdateDate
            isPrivate
            adultsOnly
            isPaid
            host {
              userId
              displayName
              photoUrl
              lastSeenOnline
              creationDate
            }
            maxNumberOfAttendees
            url
            description
            accessCode
            category
            keywords
            images
            attendees {
              userId
              displayName
              photoUrl
              lastSeenOnline
              creationDate
            }
            geoLocation {
              lat
              lng
            }
            city
            location
          }
          status {
            code
            message
          }
        }
      }
""";
}

