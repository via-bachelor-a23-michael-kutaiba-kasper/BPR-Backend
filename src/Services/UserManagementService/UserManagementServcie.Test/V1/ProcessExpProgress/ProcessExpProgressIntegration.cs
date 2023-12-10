using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using UserManagementServcie.Test.Shared;
using UserManagementService.Application.V1.ProcessExpProgress;
using UserManagementService.Application.V1.ProcessExpProgress.Data;
using UserManagementService.Application.V1.ProcessExpProgress.Model;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;
using UserManagementService.Infrastructure;

namespace UserManagementServcie.Test.V1.ProcessExpProgress;

[TestFixture]
public class ProcessExpProgressIntegration
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
    public async Task ProcessExpProgress_UserIsAwardedExp_InsertsEntriesIntoDbAndUpdatesUser()
    {
        // Arrange
        var user = "user";
        
        var ledger = new ExperienceGainedLedger();
        var loggerMock = new Mock<ILogger<ProcessExpProgressHandler>>();
        var interestSurveyMock = new Mock<IInterestSurveyRepository>();
        var progressRepository = new ProgressRepository(_connectionStringManager);
        
        interestSurveyMock.Setup(x => x.GetNewlyCreatedSurveyUserList()).ReturnsAsync(new List<string> {user});
        
        var expStrategies = new List<IExpStrategy>
        {
            new SurveyCompletedStrategy(interestSurveyMock.Object)
        };

        var testRequest = new ProcessExpProgressRequest(expStrategies);
        var handler = new ProcessExpProgressHandler(loggerMock.Object, progressRepository, ledger);
        
        // Act
        await handler.Handle(testRequest, new CancellationToken());
        var progressEntity = await GetProgressByUserId(user);
        var expProgressEntity = (await GetExpProgeressEntities(user)).ToList();
        
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(expProgressEntity.Count, Is.EqualTo(1));
            Assert.That(expProgressEntity[0].user_id, Is.EqualTo(user));
            Assert.That(expProgressEntity[0].exp_gained, Is.GreaterThan(0));
            Assert.That(progressEntity, Is.Not.Null);
            Assert.That(progressEntity?.user_id, Is.EqualTo(user));
            Assert.That(progressEntity?.total_exp, Is.GreaterThan(0));
        });

    }

    private async Task<IEnumerable<UserExpProgressEntity?>> GetExpProgeressEntities(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var query = "SELECT * FROM user_progress.user_exp_progress WHERE user_id = @userId";
        var entities = await connection.QueryAsync<UserExpProgressEntity>(query, new
        {
            @userId = userId
        });

        await connection.CloseAsync();

        return entities;
    }
    
    private async Task<ProgressEntity?> GetProgressByUserId(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();

        var query = "SELECT * FROM user_progress.progress WHERE user_id = @userId";
        var entity = await connection.QueryFirstOrDefaultAsync<ProgressEntity>(query, new
        {
            @userId = userId
        });

        await connection.CloseAsync();

        return entity;
    }
}