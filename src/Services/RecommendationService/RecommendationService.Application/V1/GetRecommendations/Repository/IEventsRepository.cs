using EventManagementService.Domain.Models.Events;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using RecommendationService.Infrastructure.ApiGateway;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface IEventsRepository
{
    public Task<IReadOnlyCollection<Event>> GetEventsWhereUserHasAttendedAsync(string userId);
}

public class EventsRepository : IEventsRepository
{
    private readonly ILogger<EventsRepository> _logger;
    private readonly IApiGateway _apiGateway;
    
    public EventsRepository(ILogger<EventsRepository> logger, IApiGateway apiGateway)
    {
        _logger = logger;
        _apiGateway = apiGateway;
    }
    
    public Task<IReadOnlyCollection<Event>> GetEventsWhereUserHasAttendedAsync(string userId)
    {
        
        throw new NotImplementedException();
    }
}