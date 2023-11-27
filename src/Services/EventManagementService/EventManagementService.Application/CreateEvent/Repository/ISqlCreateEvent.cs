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
    Task InsertEvent(Event eEvent);
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

    public async Task InsertEvent(Event eEvent)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            var locId = await InsertLocation(eEvent, connection);
            var evtId = await InsertNewEvent(eEvent, connection, locId);
            await InsertEventKeywords(eEvent, connection, evtId);
            await transaction.CommitAsync();
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

    private static async Task<int> InsertLocation(Event eEvent, NpgsqlConnection connection)
    {
        var locId = await connection.ExecuteScalarAsync<int>(
            InsertLocationSql,
            new
            {
                streetNumber = eEvent.Location.StreetNumber,
                streetName = eEvent.Location.StreetName,
                subPremise = eEvent.Location.HouseNumber,
                city = eEvent.Location.City,
                postalCode = eEvent.Location.PostalCode,
                country = eEvent.Location.Country,
                geolocationLat = eEvent.Location.GeoLocation.Lat,
                geolocationLng = eEvent.Location.GeoLocation.Lng
            });
        return locId;
    }

    private static async Task<int> InsertNewEvent(Event eEvent, NpgsqlConnection connection, int locId)
    {
        var evtId = await connection.ExecuteScalarAsync<int>(
            InsertNewEventSql,
            new
            {
                title = eEvent.Title,
                startDate = eEvent.StartDate.ToUniversalTime(),
                endDate = eEvent.EndDate.ToUniversalTime(),
                createdDate = eEvent.CreatedDate.ToUniversalTime(),
                isPrivate = eEvent.IsPrivate,
                adultOnly = eEvent.AdultsOnly,
                isPaid = eEvent.IsPaid,
                hostId = eEvent.HostId,
                maxNumberOfAttendees = eEvent.MaxNumberOfAttendees,
                lastUpdateDate = eEvent.LastUpdateDate,
                url = eEvent.Url,
                description = eEvent.Description,
                locationId = locId,
                accessCode = eEvent.AccessCode,
                categoryId = (int)eEvent.Category
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

    private const string InsertLocationSql =
        """
            INSERT INTO location
                (
                 street_number,
                 street_name,
                 sub_premise,
                 city,
                 postal_code,
                 country,
                 geolocation_lat,
                 geolocation_lng
                ) VALUES (
                          @streetNumber,
                          @streetName,
                          @subPremise,
                          @city,
                          @postalCode,
                          @country,
                          @geolocationLat,
                          @geolocationLng
                          )
                   ON CONFLICT (geolocation_lng, geolocation_lat, sub_premise)
                   DO UPDATE SET street_name =  EXCLUDED.street_name -- a hack to return id on conflict
                   RETURNING id
        """;

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
             location_id,
             access_code,
             category_id) VALUES (
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
                                  @locationId,
                                  @accessCode,
                                  @categoryId
                                ) ON CONFLICT (access_code) DO NOTHING
                                RETURNING id;
        """;

    private const string InsertEventKeywordsSql =
        """
        INSERT INTO event_keyword(event_id, keyword) VALUES(@eventId, @keyword) ON CONFLICT (keyword) DO NOTHING;
        """;
}