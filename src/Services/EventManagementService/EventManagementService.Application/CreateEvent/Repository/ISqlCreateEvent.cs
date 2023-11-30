using System.Transactions;
using Dapper;
using EventManagementService.Application.CreateEvent.Exceptions;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EventManagementService.Application.CreateEvent.Repository;

public interface ISqlCreateEvent
{
    Task<int> InsertEvent(Event eEvent);
}

public class SqlCreateEvent : ISqlCreateEvent
{
    private readonly IConnectionStringManager _connectionStringManager;
    private readonly ILogger<SqlCreateEvent> _logger;

    public SqlCreateEvent
    (
        IConnectionStringManager connectionStringManager,
        ILogger<SqlCreateEvent> logger
    )
    {
        _connectionStringManager = connectionStringManager;
        _logger = logger;
    }

    public async Task<int> InsertEvent(Event eEvent)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            var evtId = await InsertNewEvent(eEvent, connection);
            await InsertEventKeywords(eEvent, connection, evtId);
            await transaction.CommitAsync();
            return evtId;
        }
        catch (Exception e)
        {
            try
            {
                await transaction.RollbackAsync();
                _logger.LogInformation(e, "Inserting event transaction successfully rolled back");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Something went wrong while rolling back insert event transaction");
                throw new TransactionException("Cannot role back insert event transaction");
            }

            throw new InsertEventException("Cannot insert event", e);
        }
    }

    private static async Task<int> InsertNewEvent(Event eEvent, NpgsqlConnection connection)
    {
        var evtId = await connection.ExecuteScalarAsync<int>(
            InsertNewEventSql,
            new
            {
                title = eEvent.Title,
                startDate = eEvent.StartDate,
                endDate = eEvent.EndDate,
                createdDate = eEvent.CreatedDate,
                isPrivate = eEvent.IsPrivate,
                adultOnly = eEvent.AdultsOnly,
                isPaid = eEvent.IsPaid,
                hostId = eEvent.Host.UserId,
                maxNumberOfAttendees = eEvent.MaxNumberOfAttendees,
                lastUpdateDate = eEvent.LastUpdateDate,
                url = eEvent.Url,
                description = eEvent.Description,
                accessCode = eEvent.AccessCode,
                categoryId = eEvent.Category,
                location = eEvent.Location,
                city = eEvent.City,
                geolocationLat = eEvent.GeoLocation.Lat,
                geolocationLng = eEvent.GeoLocation.Lng
            });
        return evtId;
    }

    private static async Task InsertEventKeywords(Event eEvent, NpgsqlConnection connection, int evtId)
    {
        foreach (var kw in eEvent.Keywords)
        {
            await connection.ExecuteAsync(InsertEventKeywordsSql, new
            {
                eventId = evtId,
                keyword = (int)kw
            });
        }
    }

    private const string InsertNewEventSql =
        """
        INSERT INTO event
            (
             title,
             start_date,
             end_date,
             created_date,
             is_private,
             adult_only,
             is_paid,
             host_id,
             max_number_of_attendees,
             last_update_date,
             url,
             description,
             access_code,
             category_id,
             location,
             city,
             geolocation_lat,
             geolocation_lng
             ) VALUES (
                    @title,
                    @startDate,
                    @endDate,
                    @createdDate,
                    @isPrivate,
                    @adultOnly,
                    @isPaid,
                    @hostId,
                    @maxNumberOfAttendees,
                    @lastUpdateDate,
                    @url,
                    @description,
                    @accessCode,
                    @categoryId,
                    @location,
                    @city,
                    @geolocationLat,
                    @geolocationLng
                ) ON CONFLICT (access_code) DO NOTHING
                RETURNING id;
        """;

    private const string InsertEventKeywordsSql =
        """
        INSERT INTO event_keyword(event_id, keyword) VALUES(@eventId, @keyword) ON CONFLICT (keyword) DO NOTHING;
        """;
}