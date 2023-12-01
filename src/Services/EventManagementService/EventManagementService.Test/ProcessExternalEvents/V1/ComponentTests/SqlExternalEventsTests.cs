using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using EventManagementService.Application.V1.ProcessExternalEvents.Repository;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;

namespace EventManagementService.Test.ProcessExternalEvents.V1.ComponentTests;

[TestFixture]
public class SqlExternalEventsTests
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
        var repo = new SqlExternalEvents(loggerMock.Object, _connectionStringManager);
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "ProcessedExternalEvents.json"
        );

        var file = await File.ReadAllTextAsync(path);
        var events = JsonSerializer.Deserialize<List<Event>>(file, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        // Act
        var act = async () => await repo.BulkUpsertEvents(events!);

        // Assert
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }

    [Test]
    public async Task BulkInsert_ExternalEvents_InsertCorrectNumberOfRows()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SqlExternalEvents>>();
        var repo = new SqlExternalEvents(loggerMock.Object, _connectionStringManager);
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ProcessExternalEvents",
            "Fakes",
            "ProcessedExternalEvents.json"
        );

        var file = await File.ReadAllTextAsync(path);
        var events = JsonSerializer.Deserialize<List<Event>>(file, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        // Act
        await repo.BulkUpsertEvents(events!);
        var numberOfRowsInDb = GetNumberOfEvents();
        var actualNumberOfEvents = events!.Count;

        // Assert
        Assert.That(actualNumberOfEvents, Is.EqualTo(numberOfRowsInDb));
    }

    private int GetNumberOfEvents()
    {
        using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        connection.OpenAsync();
        const string sql = """
                           SELECT count(*) FROM event;
                           """;
        var nr = connection.QueryFirstOrDefault<int>(sql);
        return nr;
    }
}