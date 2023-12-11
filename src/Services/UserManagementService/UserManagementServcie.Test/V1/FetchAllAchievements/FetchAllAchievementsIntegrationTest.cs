using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using UserManagementServcie.Test.Shared;
using UserManagementService.Application.V1.FetchAllAchievements;
using UserManagementService.Application.V1.FetchAllAchievements.Repository;
using UserManagementService.Infrastructure;

namespace UserManagementServcie.Test.V1.FetchAllAchievements;

[TestFixture]
public class FetchAllAchievementsIntegrationTest
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
    public async Task FetchAllEvents_ThrowsNoExceptions()
    {
        // Arrange
        var handlerLoggerMock = new Mock<ILogger<FetchAllAchievementsHandler>>();
        var sqlRepoLoggerMock = new Mock<ILogger<SqlAchievementsRepository>>();
        var handler = new FetchAllAchievementsHandler
        (
            new SqlAchievementsRepository(sqlRepoLoggerMock.Object, _connectionStringManager),
            handlerLoggerMock.Object
        );

        // Act
        var act = async () => await handler.Handle
        (
            new FetchAllAchievementsRequest(),
            new CancellationToken()
        );

        // Assert
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }
    [Test]
    public async Task FetchAllEvents_ReturnsAllAchievements()
    {
        // Arrange
        var handlerLoggerMock = new Mock<ILogger<FetchAllAchievementsHandler>>();
        var sqlRepoLoggerMock = new Mock<ILogger<SqlAchievementsRepository>>();
        var handler = new FetchAllAchievementsHandler
        (
            new SqlAchievementsRepository(sqlRepoLoggerMock.Object, _connectionStringManager),
            handlerLoggerMock.Object
        );

        var achievementsInDbCount = await GetCountOfAchievements();

        // Act
        var achievements =  await handler.Handle
        (
            new FetchAllAchievementsRequest(),
            new CancellationToken()
        );

        // Assert
        Assert.That(achievements.Count, Is.EqualTo(achievementsInDbCount));
    }

    private async Task<int> GetCountOfAchievements()
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
         return await connection.ExecuteScalarAsync<int>("""
                                                   SELECT count(*) FROM user_progress.achievement
                                                   """);
    }
}