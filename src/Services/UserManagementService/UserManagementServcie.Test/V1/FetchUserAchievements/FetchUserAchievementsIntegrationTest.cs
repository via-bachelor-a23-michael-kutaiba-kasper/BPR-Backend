using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using UserManagementServcie.Test.Shared;
using UserManagementServcie.Test.V1.FetchUserAchievements.Model;
using UserManagementService.Application.V1.FetchUserAchievements;
using UserManagementService.Application.V1.FetchUserAchievements.Model;
using UserManagementService.Application.V1.FetchUserAchievements.Repository;
using UserManagementService.Infrastructure;

namespace UserManagementServcie.Test.V1.FetchUserAchievements;

[TestFixture]
public class FetchUserAchievementsIntegrationTest
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
    public async Task FetchUserAchievements_WithValidUserid_ThrowsNoExceptions()
    {
        // Arrange
        const string userId = "MyId";
        var handlerLoggermock = new Mock<ILogger<FetchUserAchievementsHandler>>();
        var sqlRepoLoggermock = new Mock<ILogger<SqlUserAchievementsRepository>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(x => x.UserExists(userId)).ReturnsAsync(true);
        var handler = new FetchUserAchievementsHandler
        (
            new SqlUserAchievementsRepository(sqlRepoLoggermock.Object, _connectionStringManager),
            handlerLoggermock.Object,
            userRepoMock.Object
        );
        var achievementsToAdd = new List<UserAchievements>
        {
            new()
            {
                AchievementId = 1,
                UnlocedDate = DateTimeOffset.UtcNow.AddDays(-2).ToUniversalTime(),
                UserId = userId
            },
            new()
            {
                AchievementId = 5,
                UnlocedDate = DateTimeOffset.UtcNow.AddDays(-1).ToUniversalTime(),
                UserId = userId
            }
        };

        await InsertAchievements(achievementsToAdd);

        // Act
        var act = async () => await handler.Handle(new FetchUserAchievementsRequest(userId), new CancellationToken());

        // Assert
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }

    [Test]
    public async Task FetchUserAchievements_WithValidUserid_ReturnsUSerAchievements()
    {
        // Arrange
        const string userId = "MyId";
        var handlerLoggermock = new Mock<ILogger<FetchUserAchievementsHandler>>();
        var sqlRepoLoggermock = new Mock<ILogger<SqlUserAchievementsRepository>>();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(x => x.UserExists(userId)).ReturnsAsync(true);
        var handler = new FetchUserAchievementsHandler
        (
            new SqlUserAchievementsRepository(sqlRepoLoggermock.Object, _connectionStringManager),
            handlerLoggermock.Object,
            userRepoMock.Object
        );
        var achievementsToAdd = new List<UserAchievements>
        {
            new()
            {
                AchievementId = 1,
                UnlocedDate = DateTimeOffset.UtcNow.AddDays(-2).ToUniversalTime(),
                UserId = userId
            },
            new()
            {
                AchievementId = 5,
                UnlocedDate = DateTimeOffset.UtcNow.AddDays(-1).ToUniversalTime(),
                UserId = userId
            }
        };

        await InsertAchievements(achievementsToAdd);

        // Act
        var handle = await handler.Handle(new FetchUserAchievementsRequest(userId), new CancellationToken());
        var expected = handle.ToList();

        // Assert
        Assert.That(achievementsToAdd.Count, Is.EqualTo(expected.Count));
    }

    private async Task InsertAchievements(IReadOnlyCollection<UserAchievements> tables)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        const string sql =
            """
            INSERT INTO user_progress.user_achievement(achievement_id, user_id, unlocked_date)
            VALUES(@AchievementId, @UserId, @UnlocedDate)
            """;
        foreach (var table in tables)
        {
            await connection.ExecuteAsync(sql, new
            {
                AchievementId = table.AchievementId,
                UserId = table.UserId,
                UnlocedDate = table.UnlocedDate
            });
        }
    }
}