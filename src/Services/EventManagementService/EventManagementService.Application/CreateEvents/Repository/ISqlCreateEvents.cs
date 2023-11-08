using Dapper;
using EventManagementService.Application.CreateEvents.Exceptions;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EventManagementService.Application.CreateEvents.Repository;

public interface ISqlCreateEvents
{
    Task UpsertEvents(IReadOnlyCollection<Event> events);
}

public class SqlCreateEvents : ISqlCreateEvents
{
    private readonly IOptions<ConnectionStrings> _options;
    private readonly ILogger<SqlCreateEvents> _logger;

    public SqlCreateEvents(ILogger<SqlCreateEvents> logger, IOptions<ConnectionStrings> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task UpsertEvents(IReadOnlyCollection<Event> events)
    {
        throw new NotImplementedException();
    }

    private static string InsertEventSql()
    {
        //TODO: update this insert -> look into temp tables to insert and then use merge to copy data using binary copy
        //TODO: add new migration for access code
        return """
               INSERT INTO public.event(title,url,location,description)
               values (@title, @url, @location, @description)
               """;
    }
}