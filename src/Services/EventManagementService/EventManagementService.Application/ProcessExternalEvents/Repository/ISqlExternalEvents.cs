using System.Transactions;
using Dapper;
using EventManagementService.Application.ProcessExternalEvents.Exceptions;
using EventManagementService.Application.ProcessExternalEvents.Sql;
using EventManagementService.Application.ProcessExternalEvents.Util;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.AppSettings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace EventManagementService.Application.ProcessExternalEvents.Repository;

public interface ISqlExternalEvents
{
    Task BulkUpsertEvents(IReadOnlyCollection<Event> events);
}

public class SqlExternalEvents : ISqlExternalEvents
{
    private readonly IOptions<ConnectionStrings> _options;
    private readonly ILogger<SqlExternalEvents> _logger;

    public SqlExternalEvents(ILogger<SqlExternalEvents> logger, IOptions<ConnectionStrings> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task BulkUpsertEvents(IReadOnlyCollection<Event> events)
    {
        await using var connection = new NpgsqlConnection(_options.Value.Postgres);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        try
        {
            await CreateTempTables(connection);
            await InsertImportedData(connection, events);
            await UpsertData(connection, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            try
            {
                await transaction.RollbackAsync();
                _logger.LogInformation(e, "Upsert events transaction successfully rolled back");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Something went wrong while rolling back upsert events transaction");
                throw new TransactionException("Cannot role back upsert events transaction");
            }

            throw new UpsertEventsException("Cannot upsert events", e);
        }
    }

    private static async Task CreateTempTables(NpgsqlConnection connection)
    {
        await connection.ExecuteAsync(SqlCommands.CreateTempTables);
    }

    private static async Task InsertImportedData
    (
        NpgsqlConnection connection,
        IReadOnlyCollection<Event> events
    )
    {
        await InsertImportedLocationInTemp(connection, events);
        await InsertImportedEventsTemp(connection, events);
        await InsertImportedImagesTemp(connection, events);
        await InsertImportedEventKeywordsTemp(connection, events);
    }

    private static async Task UpsertData(NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        await connection.ExecuteAsync(SqlCommands.UpsertLocations, null, transaction);
        await connection.ExecuteAsync(SqlCommands.UpsertEvents, null, transaction);
        await connection.ExecuteAsync(SqlCommands.UpsertImage, null, transaction);
        await connection.ExecuteAsync(SqlCommands.UpsertEventKeyword, null, transaction);
    }

    private static async Task InsertImportedLocationInTemp
    (
        NpgsqlConnection connection,
        IReadOnlyCollection<Event> events
    )
    {
        using (var writer = await connection.BeginBinaryImportAsync(SqlCommands.ImportLocationBinaryCopy))
        {
            foreach (var et in events)
            {
                await writer.StartRowAsync();
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Location.StreetNumber,
                    NpgsqlDbType.Varchar);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Location.StreetName,
                    NpgsqlDbType.Varchar);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Location.City, NpgsqlDbType.Varchar);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Location.PostalCode,
                    NpgsqlDbType.Varchar);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Location.Country, NpgsqlDbType.Varchar);
                await writer.WriteAsync(et.Location.GeoLocation.Lat, NpgsqlDbType.Numeric);
                await writer.WriteAsync(et.Location.GeoLocation.Lng, NpgsqlDbType.Numeric);
                await writer.CompleteAsync();
            }
        }
    }

    private static async Task InsertImportedEventsTemp
    (
        NpgsqlConnection connection,
        IReadOnlyCollection<Event> events
    )
    {
        using (var writer = await connection.BeginBinaryImportAsync(SqlCommands.ImportLocationBinaryCopy))
        {
            foreach (var et in events)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(et.Title, NpgsqlDbType.Varchar);
                await writer.WriteAsync(et.StartDate.ToUniversalTime(), NpgsqlDbType.TimestampTz);
                await writer.WriteAsync(et.EndDate.ToUniversalTime(), NpgsqlDbType.TimestampTz);
                await writer.WriteAsync(et.CreatedDate.ToUniversalTime(), NpgsqlDbType.TimestampTz);
                await writer.WriteAsync(et.IsPrivate, NpgsqlDbType.Boolean);
                await writer.WriteAsync(et.IsPaid, NpgsqlDbType.Boolean);
                await writer.WriteAsync(et.HostId, NpgsqlDbType.Varchar);
                await writer.WriteAsync(et.AccessCode, NpgsqlDbType.Varchar);
                await BinaryWriterHelper<int>.WriteNullableAsync(writer, et.MaxNumberOfAttendees, NpgsqlDbType.Integer);
                await BinaryWriterHelper<DateTimeOffset>.WriteNullableAsync(writer, et.LastUpdateDate.ToUniversalTime(),
                    NpgsqlDbType.TimestampTz);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Url, NpgsqlDbType.Varchar);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Description, NpgsqlDbType.Varchar);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Location.HouseNumber,
                    NpgsqlDbType.Varchar);
                await BinaryWriterHelper<float>.WriteNullableAsync(writer, et.Location.GeoLocation.Lat,
                    NpgsqlDbType.Numeric);
                await BinaryWriterHelper<float>.WriteNullableAsync(writer, et.Location.GeoLocation.Lng,
                    NpgsqlDbType.Numeric);
                await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.AccessCode,
                    NpgsqlDbType.Varchar);
                await BinaryWriterHelper<int>.WriteNullableAsync(writer, (int)et.Category,
                    NpgsqlDbType.Integer);
                await writer.CompleteAsync();
            }
        }
    }

    private static async Task InsertImportedImagesTemp
    (
        NpgsqlConnection connection,
        IReadOnlyCollection<Event> events
    )
    {
        using (var writer = await connection.BeginBinaryImportAsync(SqlCommands.ImportLocationBinaryCopy))
        {
            foreach (var et in events)
            {
                foreach (var image in et.Images)
                {
                    await writer.StartRowAsync();
                    await BinaryWriterHelper<string>.WriteNullableAsync(writer, image,
                        NpgsqlDbType.Varchar);
                    await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Url,
                        NpgsqlDbType.Numeric);
                    await writer.CompleteAsync();
                }
            }
        }
    }

    private static async Task InsertImportedEventKeywordsTemp
    (
        NpgsqlConnection connection,
        IReadOnlyCollection<Event> events
    )
    {
        using (var writer = await connection.BeginBinaryImportAsync(SqlCommands.ImportLocationBinaryCopy))
        {
            foreach (var et in events)
            {
                foreach (var keyword in et.Keywords)
                {
                    await writer.StartRowAsync();
                    await BinaryWriterHelper<int>.WriteNullableAsync(writer, (int)keyword,
                        NpgsqlDbType.Integer);
                    await BinaryWriterHelper<string>.WriteNullableAsync(writer, et.Url,
                        NpgsqlDbType.Numeric);
                    await writer.CompleteAsync();
                }
            }
        }
    }
}