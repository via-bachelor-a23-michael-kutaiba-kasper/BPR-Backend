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
using UserManagementService.Infrastructure.Util;

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
    public async Task AchievementProcess_AfterUserJoinedNewEventAndIsValid_ThrowsNoException()
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

        var sqlRepo = new SqlAchievementRepository(_connectionStringManager, sqlLoggerMock.Object);
        var strategies = new List<CheckAchievementBaseStrategy>();
        strategies.AddRange(new CheckAchievementBaseStrategy[]
        {
            new CheckCulinaryAndDrinksStrategy(sqlRepo),
            new CheckCulturalAndArtisticStrategy(sqlRepo),
            new CheckHealthAndWellness(sqlRepo),
            new CheckLearningAndDevelopmentStrategy(sqlRepo),
            new CheckMusicAndPerformingArtsStrategy(sqlRepo),
            new CheckRecreationAndHobbiesStrategy(sqlRepo),
            new CheckSocialAndCommunityStrategy(sqlRepo),
            new CheckUserSurveyAchievement(sqlRepo, eventBussMockStrategy.Object, _options)
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
            sqlRepo,
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

    [TestCase(UserAchievement.Canary1, 4)]
    [TestCase(UserAchievement.Canary2, 4)]
    [TestCase(UserAchievement.Canary3, 4)]
    [TestCase(UserAchievement.Peacock1, 4)]
    [TestCase(UserAchievement.Peacock2, 4)]
    [TestCase(UserAchievement.Peacock3, 4)]
    [TestCase(UserAchievement.Butterfly1, 4)]
    [TestCase(UserAchievement.Butterfly2, 4)]
    [TestCase(UserAchievement.Butterfly3, 4)]
    [TestCase(UserAchievement.Bear1, 4)]
    [TestCase(UserAchievement.Bear2, 4)]
    [TestCase(UserAchievement.Bear3, 4)]
    [TestCase(UserAchievement.Monkey1, 4)]
    [TestCase(UserAchievement.Monkey2, 4)]
    [TestCase(UserAchievement.Monkey3, 4)]
    [TestCase(UserAchievement.Owl1, 4)]
    [TestCase(UserAchievement.Owl2, 4)]
    [TestCase(UserAchievement.Owl3, 4)]
    [TestCase(UserAchievement.Cheetah1, 4)]
    [TestCase(UserAchievement.Cheetah2, 4)]
    [TestCase(UserAchievement.Cheetah3, 4)]
    [TestCase(UserAchievement.NewComer, 4)]
    public async Task TheCorrectAchievementUnlocks_WithValidTier1_UnlockesOnyTier1Achievements(
        UserAchievement achievement, int oldProgress)
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

        var sqlRepo = new SqlAchievementRepository(_connectionStringManager, sqlLoggerMock.Object);
        var strategies = new List<CheckAchievementBaseStrategy>();
        strategies.AddRange(new CheckAchievementBaseStrategy[]
        {
            new CheckCulinaryAndDrinksStrategy(sqlRepo),
            new CheckCulturalAndArtisticStrategy(sqlRepo),
            new CheckHealthAndWellness(sqlRepo),
            new CheckLearningAndDevelopmentStrategy(sqlRepo),
            new CheckMusicAndPerformingArtsStrategy(sqlRepo),
            new CheckRecreationAndHobbiesStrategy(sqlRepo),
            new CheckSocialAndCommunityStrategy(sqlRepo),
            new CheckUserSurveyAchievement(sqlRepo, eventBussMockStrategy.Object, _options)
        });

        await MockUserProgress(new UnlockableAchievementProgressTable
        {
            achievement_id = (int)achievement,
            date = DateTimeOffset.UtcNow.AddDays(-1).ToUniversalTime(),
            user_id = userId,
            progress = oldProgress
        });

        var handler = new ProcessUserAchievementsHandle
        (
            sqlRepo,
            userRepoMock.Object,
            handlerLoggerMock.Object,
            strategies.ToList(),
            notifierMock.Object,
            eventBussMock.Object,
            _options
        );

        // Act
        await handler.Handle(new ProcessUserAchievementsRequest(userId), ct);
        var unlocked = await GetUnlockedAchievements(userId, (int)achievement);

        // Assert
        if (achievement.GetDescription().Contains('1') || achievement == UserAchievement.NewComer)
        {
            Assert.That((int)achievement, Is.EqualTo(unlocked));
        }
        else
        {
            Assert.That((int)achievement, Is.Not.EqualTo(unlocked));
        }
    }

    [TestCase(UserAchievement.Canary1, 20)]
    [TestCase(UserAchievement.Canary2, 20)]
    [TestCase(UserAchievement.Canary3, 20)]
    [TestCase(UserAchievement.Peacock1, 20)]
    [TestCase(UserAchievement.Peacock2, 20)]
    [TestCase(UserAchievement.Peacock3, 20)]
    [TestCase(UserAchievement.Butterfly1, 20)]
    [TestCase(UserAchievement.Butterfly2, 20)]
    [TestCase(UserAchievement.Butterfly3, 20)]
    [TestCase(UserAchievement.Bear1, 20)]
    [TestCase(UserAchievement.Bear2, 20)]
    [TestCase(UserAchievement.Bear3, 20)]
    [TestCase(UserAchievement.Monkey1, 20)]
    [TestCase(UserAchievement.Monkey2, 20)]
    [TestCase(UserAchievement.Monkey3, 20)]
    [TestCase(UserAchievement.Owl1, 20)]
    [TestCase(UserAchievement.Owl2, 20)]
    [TestCase(UserAchievement.Owl3, 20)]
    [TestCase(UserAchievement.Cheetah1, 20)]
    [TestCase(UserAchievement.Cheetah2, 20)]
    [TestCase(UserAchievement.Cheetah3, 20)]
    [TestCase(UserAchievement.NewComer, 20)]
    public async Task TheCorrectAchievementUnlocks_WithValidTier2_UnlockesOnyTier2Achievements(
        UserAchievement achievement, int oldProgress)
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

        var sqlRepo = new SqlAchievementRepository(_connectionStringManager, sqlLoggerMock.Object);
        var strategies = new List<CheckAchievementBaseStrategy>();
        strategies.AddRange(new CheckAchievementBaseStrategy[]
        {
            new CheckCulinaryAndDrinksStrategy(sqlRepo),
            new CheckCulturalAndArtisticStrategy(sqlRepo),
            new CheckHealthAndWellness(sqlRepo),
            new CheckLearningAndDevelopmentStrategy(sqlRepo),
            new CheckMusicAndPerformingArtsStrategy(sqlRepo),
            new CheckRecreationAndHobbiesStrategy(sqlRepo),
            new CheckSocialAndCommunityStrategy(sqlRepo),
            new CheckUserSurveyAchievement(sqlRepo, eventBussMockStrategy.Object, _options)
        });

        await MockUserProgress(new UnlockableAchievementProgressTable
        {
            achievement_id = (int)achievement,
            date = DateTimeOffset.UtcNow.AddDays(-1).ToUniversalTime(),
            user_id = userId,
            progress = oldProgress
        });

        var handler = new ProcessUserAchievementsHandle
        (
            sqlRepo,
            userRepoMock.Object,
            handlerLoggerMock.Object,
            strategies.ToList(),
            notifierMock.Object,
            eventBussMock.Object,
            _options
        );

        // Act
        await handler.Handle(new ProcessUserAchievementsRequest(userId), ct);
        var unlocked = await GetUnlockedAchievements(userId, (int)achievement);

        // Assert

        if (achievement.GetDescription().Contains('2') || achievement.GetDescription().Contains('1') ||
            achievement == UserAchievement.NewComer)
        {
            Assert.That((int)achievement, Is.EqualTo(unlocked));
        }

        if (achievement.GetDescription().Contains('3'))
        {
            Assert.That((int)achievement, Is.Not.EqualTo(unlocked));
        }
    }

    [TestCase(UserAchievement.Canary1, 50)]
    [TestCase(UserAchievement.Canary2, 50)]
    [TestCase(UserAchievement.Canary3, 50)]
    [TestCase(UserAchievement.Peacock1, 50)]
    [TestCase(UserAchievement.Peacock2, 50)]
    [TestCase(UserAchievement.Peacock3, 50)]
    [TestCase(UserAchievement.Butterfly1, 50)]
    [TestCase(UserAchievement.Butterfly2, 50)]
    [TestCase(UserAchievement.Butterfly3, 50)]
    [TestCase(UserAchievement.Bear1, 50)]
    [TestCase(UserAchievement.Bear2, 50)]
    [TestCase(UserAchievement.Bear3, 50)]
    [TestCase(UserAchievement.Monkey1, 50)]
    [TestCase(UserAchievement.Monkey2, 50)]
    [TestCase(UserAchievement.Monkey3, 50)]
    [TestCase(UserAchievement.Owl1, 50)]
    [TestCase(UserAchievement.Owl2, 50)]
    [TestCase(UserAchievement.Owl3, 50)]
    [TestCase(UserAchievement.Cheetah1, 50)]
    [TestCase(UserAchievement.Cheetah2, 50)]
    [TestCase(UserAchievement.Cheetah3, 50)]
    [TestCase(UserAchievement.NewComer, 50)]
    public async Task TheCorrectAchievementUnlocks_WithValidTier3_UnlockesOnyTier3Achievements(
        UserAchievement achievement, int oldProgress)
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

        var sqlRepo = new SqlAchievementRepository(_connectionStringManager, sqlLoggerMock.Object);
        var strategies = new List<CheckAchievementBaseStrategy>();
        strategies.AddRange(new CheckAchievementBaseStrategy[]
        {
            new CheckCulinaryAndDrinksStrategy(sqlRepo),
            new CheckCulturalAndArtisticStrategy(sqlRepo),
            new CheckHealthAndWellness(sqlRepo),
            new CheckLearningAndDevelopmentStrategy(sqlRepo),
            new CheckMusicAndPerformingArtsStrategy(sqlRepo),
            new CheckRecreationAndHobbiesStrategy(sqlRepo),
            new CheckSocialAndCommunityStrategy(sqlRepo),
            new CheckUserSurveyAchievement(sqlRepo, eventBussMockStrategy.Object, _options)
        });

        await MockUserProgress(new UnlockableAchievementProgressTable
        {
            achievement_id = (int)achievement,
            date = DateTimeOffset.UtcNow.AddDays(-1).ToUniversalTime(),
            user_id = userId,
            progress = oldProgress
        });

        var handler = new ProcessUserAchievementsHandle
        (
            sqlRepo,
            userRepoMock.Object,
            handlerLoggerMock.Object,
            strategies.ToList(),
            notifierMock.Object,
            eventBussMock.Object,
            _options
        );

        // Act
        await handler.Handle(new ProcessUserAchievementsRequest(userId), ct);
        var unlocked = await GetUnlockedAchievements(userId, (int)achievement);

        // Assert

        if (achievement.GetDescription().Contains('2') || achievement.GetDescription().Contains('1') ||
            achievement.GetDescription().Contains('3') ||
            achievement == UserAchievement.NewComer)
        {
            Assert.That((int)achievement, Is.EqualTo(unlocked));
        }
    }

    [TestCase(UserAchievement.Canary1, 1)]
    [TestCase(UserAchievement.Canary2, 1)]
    [TestCase(UserAchievement.Canary3, 1)]
    [TestCase(UserAchievement.Peacock1, 1)]
    [TestCase(UserAchievement.Peacock2, 1)]
    [TestCase(UserAchievement.Peacock3, 1)]
    [TestCase(UserAchievement.Butterfly1, 1)]
    [TestCase(UserAchievement.Butterfly2, 1)]
    [TestCase(UserAchievement.Butterfly3, 1)]
    [TestCase(UserAchievement.Bear1, 1)]
    [TestCase(UserAchievement.Bear2, 1)]
    [TestCase(UserAchievement.Bear3, 1)]
    [TestCase(UserAchievement.Monkey1, 1)]
    [TestCase(UserAchievement.Monkey2, 1)]
    [TestCase(UserAchievement.Monkey3, 1)]
    [TestCase(UserAchievement.Owl1, 1)]
    [TestCase(UserAchievement.Owl2, 1)]
    [TestCase(UserAchievement.Owl3, 1)]
    [TestCase(UserAchievement.Cheetah1, 1)]
    [TestCase(UserAchievement.Cheetah2, 1)]
    [TestCase(UserAchievement.Cheetah3, 1)]
    public async Task InsufficientProgress_UpdateProgress_ForTier1Only(UserAchievement achievement, int oldProgress)
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

        var sqlRepo = new SqlAchievementRepository(_connectionStringManager, sqlLoggerMock.Object);
        var strategies = new List<CheckAchievementBaseStrategy>();
        strategies.AddRange(new CheckAchievementBaseStrategy[]
        {
            new CheckCulinaryAndDrinksStrategy(sqlRepo),
            new CheckCulturalAndArtisticStrategy(sqlRepo),
            new CheckHealthAndWellness(sqlRepo),
            new CheckLearningAndDevelopmentStrategy(sqlRepo),
            new CheckMusicAndPerformingArtsStrategy(sqlRepo),
            new CheckRecreationAndHobbiesStrategy(sqlRepo),
            new CheckSocialAndCommunityStrategy(sqlRepo),
            new CheckUserSurveyAchievement(sqlRepo, eventBussMockStrategy.Object, _options)
        });

        await MockUserProgress(new UnlockableAchievementProgressTable
        {
            achievement_id = (int)achievement,
            date = DateTimeOffset.UtcNow.AddDays(-1).ToUniversalTime(),
            user_id = userId,
            progress = oldProgress
        });

        var handler = new ProcessUserAchievementsHandle
        (
            sqlRepo,
            userRepoMock.Object,
            handlerLoggerMock.Object,
            strategies.ToList(),
            notifierMock.Object,
            eventBussMock.Object,
            _options
        );

        // Act
        await handler.Handle(new ProcessUserAchievementsRequest(userId), ct);
        var progressedAchievement = await GetProgressedAchievements(userId, (int)achievement);
        var progress = await GetProgressForAchievements(userId, (int)achievement);

        // Assert
        if (achievement.GetDescription().Contains('1'))
        {
            Assert.That((int)achievement, Is.EqualTo(progressedAchievement));
            Assert.That(oldProgress + 1 , Is.EqualTo(progress));
        }

        if (achievement.GetDescription().Contains('1') ||
            achievement.GetDescription().Contains('3'))
        {
            Assert.That((int)achievement, Is.Not.EqualTo(progress));
        }
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

    private async Task<int> GetUnlockedAchievements(string userId, int achievementId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        const string sql =
            """
            SELECT achievement_id FROM user_achievement WHERE user_id = @UserId AND achievement_id = @AchievementId;
            """;
        var query = await connection.QueryFirstOrDefaultAsync<int>(sql,
            new { UserId = userId, AchievementId = achievementId });
        return query;
    }

    private async Task<int> GetProgressedAchievements(string userId, int achievementId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        const string sql =
            """
            SELECT achievement_id FROM unlockable_achievement_progress WHERE user_id = @UserId AND achievement_id = @AchievementId;
            """;
        var query = await connection.QueryFirstOrDefaultAsync<int>(sql,
            new { UserId = userId, AchievementId = achievementId });
        return query;
    }
    private async Task<int> GetProgressForAchievements(string userId, int achievementId)
    {
        await using var connection = new NpgsqlConnection(_connectionStringManager.GetConnectionString());
        await connection.OpenAsync();
        const string sql =
            """
            SELECT progress FROM unlockable_achievement_progress WHERE user_id = @UserId AND achievement_id = @AchievementId;
            """;
        var query = await connection.QueryFirstOrDefaultAsync<int>(sql,
            new { UserId = userId, AchievementId = achievementId });
        return query;
    }
}