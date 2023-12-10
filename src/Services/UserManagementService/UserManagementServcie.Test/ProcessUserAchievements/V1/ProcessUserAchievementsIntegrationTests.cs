using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;
using UserManagementServcie.Test.Shared;
using UserManagementServcie.Test.Shared.Builders;
using UserManagementService.Application.V1.ProcessUserAchievements;
using UserManagementService.Application.V1.ProcessUserAchievements.Model;
using UserManagementService.Application.V1.ProcessUserAchievements.Model.Strategy;
using UserManagementService.Application.V1.ProcessUserAchievements.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.AppSettings;
using UserManagementService.Infrastructure.Notifications;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementServcie.Test.ProcessUserAchievements.V1;

[TestFixture]
public class ProcessUserAchievementsIntegrationTests
{
    private readonly TestDataContext _context = new();
    private readonly ConnectionStringManager _connectionStringManager = new();

    private readonly IOptions<PubSub> _options = Options.Create(new PubSub
    {
        Topics = new[]
        {
            new Topic
            {
                TopicId = "Test",
                ProjectId = "Test",
                SubscriptionNames = new[]
                {
                    "Test"
                }
            },
            new Topic
            {
                TopicId = "Test",
                ProjectId = "Test",
                SubscriptionNames = new[]
                {
                    "Test"
                }
            }
        }
    });

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
    public async Task TheCorrectAchievementUnlocks_AfterUserJoinedNewEventAndIsValid_ThrowsNoException()
    {
        // Arrange
        const string userId = "MyId";
        var sqlLoggerMock = new Mock<ILogger<SqlAchievementRepository>>();
        var userLoggerMock = new Mock<ILogger<UserRepository>>();
        var handlerLoggerMock = new Mock<ILogger<ProcessUserAchievementsHandle>>();
        var notifierMock = new Mock<INotifier>();
        var eventBussMock = new Mock<IEventBus>();
        var eventBussMockStrategy = new Mock<IEventBus>();
        var userRepoMock = new Mock<IUserRepository>();
        var eventBuilder = new EventBuilder();

        var joinedEvents = new List<Event>();
        var ev = eventBuilder.WithRequiredFields
        (
            "Test",
            DateTimeOffset.UtcNow.AddHours(2).ToUniversalTime(),
            DateTimeOffset.UtcNow.AddHours(4).ToUniversalTime(),
            DateTimeOffset.UtcNow.ToUniversalTime(),
            false, true, false,
            "hostId", "TestAccessCode",
            Category.Music, "location", "city",
            0, 0
        ).Build();
        joinedEvents.Add(ev);

        var ct = new CancellationToken();
        eventBussMock.Setup(x => x.PullAsync<Event>
        (
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(),
            ct
        )).ReturnsAsync(joinedEvents);

        eventBussMockStrategy.Setup(x => x.PullAsync<string>
        (
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<int>(),
            ct
        )).ReturnsAsync(new[] { userId });

        userRepoMock.Setup(x => x.UserExists(userId)).ReturnsAsync(true);
        userRepoMock.Setup(x => x.GetNotificationTokenByUserIdAsync(userId)).ReturnsAsync("MyToken");

        var strategies = new List<CheckAchievementBaseStrategy>();
        strategies.AddRange(new CheckAchievementBaseStrategy[]
        {
            new CheckCulinaryAndDrinksStrategy(),
            new CheckCulturalAndArtisticStrategy(),
            new CheckHealthAndWellness(),
            new CheckLearningAndDevelopmentStrategy(),
            new CheckMusicAndPerformingArtsStrategy(),
            new CheckRecreationAndHobbiesStrategy(),
            new CheckSocialAndCommunityStrategy(),
            new CheckUserSurveyAchievement(eventBussMockStrategy.Object, _options)
        });

        await MockUserProgress(new UnlockableAchievementProgressTable
        {
            achievement_id = (int)UserAchievement.Canary1,
            date = DateTimeOffset.UtcNow.AddDays(-1).ToUniversalTime(),
            user_id = userId,
            progress = 4
        });

        var handler = new ProcessUserAchievementsHandle
        (
            new SqlAchievementRepository(_connectionStringManager, sqlLoggerMock.Object),
            userRepoMock.Object,
            handlerLoggerMock.Object,
            strategies.ToList(),
            notifierMock.Object,
            eventBussMock.Object,
            _options
        );

        // Act
        var act = async () => await handler.Handle(new ProcessUserAchievementsRequest(userId), ct);

        // Assert
        Assert.DoesNotThrowAsync(() => act.Invoke());
    }

    private async Task MockUserProgress(UnlockableAchievementProgressTable table)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        const string sql =
            """
            INSERT INTO unlockable_achievement_progress(achievement_id, user_id, progress, date)
            VALUES(@achievement_id, @user_id, @progress, @date)
            """;
        await connection.ExecuteAsync(sql, new
        {
            achievement_id = table.achievement_id,
            user_id = table.user_id,
            progress = table.progress,
            date = table.date
        });
    }
}