using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using UserManagementServcie.Test.Shared;
using UserManagementService.Application.V1.GetUserExpProgres;
using UserManagementService.Application.V1.GetUserExpProgres.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure;

namespace UserManagementServcie.Test.V1.GetUserExpProgress;

[TestFixture]
public class GetUserExpProgressIntegratioon
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
    public async Task GetUserExpProgress_UserExistsAndHasProgress_RetrievesUserProgressCorrectly()
    {
        // Arrange
        var user = "user";
        
        var loggerMock = new Mock<ILogger<GetUserExpProgressHandler>>();
        var logger2Mock = new Mock<ILogger<LevelRepository>>();
        var logger3Mock = new Mock<ILogger<ProgressRepository>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var levelRepository = new LevelRepository(logger2Mock.Object, _connectionStringManager);
        var progressRepository = new ProgressRepository(logger3Mock.Object, _connectionStringManager);

        userRepositoryMock.Setup(x => x.UserExistsAsync(user)).ReturnsAsync(true);

        await InsertProgress(user, new Progress
        {
            Stage = -1,
            TotalExp = 2700
        });

        var handler = new GetUserExpProgressHandler(loggerMock.Object, progressRepository,
            userRepositoryMock.Object, levelRepository);
        var request = new GetUserExpProgressRequest(user);
        
        // Act
        var progress = await handler.Handle(request, new CancellationToken());
        
        // Assert
        Assert.That(progress.TotalExp, Is.EqualTo(2700));
        Assert.That(progress.Level.Name.ToLower(), Is.EqualTo("social caterpillar"));
        Assert.That(progress.Stage, Is.EqualTo(2));
    }

    public async Task InsertProgressEntry(string userId, ExpProgressEntry entry)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var statement =
            "INSERT INTO user_progress.user_exp_progress(user_id, exp_gained, datetime) VALUES (@userId, @expGained, @datetime)";

        await connection.ExecuteAsync(statement, new
        {
            @userId = userId,
            @expGained = entry.ExpGained,
            @datetime = entry.Timestamp
        });

        await connection.CloseAsync();
    }

    public async Task InsertProgress(string userId, Progress progress)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var statement =
            "INSERT INTO user_progress.progress(user_id, total_exp, stage) VALUES (@userId, @totalExp, @stage)";
        await connection.ExecuteAsync(statement, new
        {
            @userId = userId,
            @totalExp = progress.TotalExp,
            @stage = progress.Stage
        });

        await connection.CloseAsync();
    }
}