using Dapper;
using EventManagementService.Application.FetchAllEvents.Data;
using EventManagementService.Application.FetchAllEvents.Model;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.FetchAllEvents.Repository;

public interface ISqlAllEvents
{
    Task<IReadOnlyCollection<Event>> GetAllEvents(Filters filters);
}

public class SqlAllEvents : ISqlAllEvents
{
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly ILogger<SqlAllEvents> _logger;

    public SqlAllEvents
    (
        IConnectionStringManager connectionStringManager,
        ILogger<SqlAllEvents> logger
    )
    {
        _connectionStringManager = connectionStringManager;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Event>> GetAllEvents(Filters filters)
    {
        List<Event> events = new();
        using (var connection = new NpgsqlConnection())
        {
            await connection.OpenAsync();
            var queryEventStatement = $"SELECT * from event {new ApplyFiltersInSql().Apply(filters)}";
            var queryParams = new
            {
                @from = filters.From,
                @to = filters.To,
                @hostId = filters.HostId
            };
            
            var eventEntities =
                await connection.QueryAsync<EventEntity>(SqlQueries.QueryAllFromEventTable, queryParams) ??
                new List<EventEntity>();

            var eventIds = eventEntities.Select(e => e.id);
            var indexedKeywords = GetIndexedKeywordsByEventId(connection, eventIds.ToList());

            var domainEvents = eventEntities.Select(e => new Event
            {
                
            });
        }

        return events;
    }

    private async Task<IDictionary<int, List<int>>> GetIndexedKeywordsByEventId(NpgsqlConnection connection,
        IReadOnlyCollection<int> eventIds)
    {
            var queryEventKeywords = $"SELECT * from event_keyword WHERE event_id in {SqlUtil.AsIntList(eventIds.ToList())}";
            var eventKeywordEntities = await connection.QueryAsync<EventKeywordEntity>(queryEventKeywords) ?? new List<EventKeywordEntity>();
            return IndexKeywordsByEventId(eventKeywordEntities.ToList());
    }
    
    private IDictionary<int, List<int>> IndexKeywordsByEventId(IReadOnlyCollection<EventKeywordEntity> eventKeywordEntities)
    {
        Dictionary<int, List<int>> indexedKeywords = new();

        foreach (var entity in eventKeywordEntities)
        {
            if (!indexedKeywords.ContainsKey(entity.event_id))
            {
                indexedKeywords[entity.event_id] = new List<int>() { entity.keyword };
            }
            else
            {
                indexedKeywords[entity.event_id].Add(entity.keyword);
            }
        }

        return indexedKeywords;
    }
}