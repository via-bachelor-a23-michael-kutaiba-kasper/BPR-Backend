using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using UserManagementServcie.Test.Shared.Builders;
using UserManagementService.Application.V1.ProcessExpProgress;
using UserManagementService.Application.V1.ProcessExpProgress.Dtos;
using UserManagementService.Application.V1.ProcessExpProgress.Model;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure.AppSettings;

namespace UserManagementServcie.Test.V1.ProcessExpProgress;

[TestFixture]
public class ProcessExpProgressTests
{
    private readonly IOptions<PubSub> _pubsubConfig = Options.Create(new PubSub()
    {
        Topics = new[]
        {
            new Topic()
            {
                ProjectId = "test",
                TopicId = "test",
                SubscriptionNames = new[]
                {
                    "test",
                    "test"
                }
            }
        }
    });

    [Test]
    public async Task ProcessExpProgress_JoinEvents_AwardsExpToHostAndUser()
    {
        // Arrange
        var hostId = "host";
        var attendeeId1 = "attendee1";
        var attendeeId2 = "attendee2";
        var ledger = new ExperienceGainedLedger();

        var attendeesRepositoryMock = new Mock<IAttendeesRepository>();
        var progressRepositoryMock = new Mock<IProgressRepository>();
        var loggerMock = new Mock<ILogger<ProcessExpProgressHandler>>();

        attendeesRepositoryMock.Setup(x => x.GetNewEventAttendees()).ReturnsAsync(new List<Attendance>
        {
            new Attendance
            {
                Event = new Event
                {
                    Host = new User {UserId = hostId}
                },
                UserId = attendeeId1
            },
            new Attendance
            {
                Event = new Event
                {
                    Host = new User {UserId = hostId}
                },
                UserId = attendeeId2
            }
        });

        var expStrategies = new List<IExpStrategy>
        {
            new NewAttendeesStrategy(attendeesRepositoryMock.Object)
        };

        var testRequest = new ProcessExpProgressRequest(expStrategies);
        var handler = new ProcessExpProgressHandler(loggerMock.Object, progressRepositoryMock.Object, ledger);

        // Act
        await handler.Handle(testRequest, new CancellationToken());

        // Assert
        Assert.That(ledger.GetExperienceGained(hostId), Is.EqualTo(800));
        Assert.That(ledger.GetExperienceGained(attendeeId1), Is.EqualTo(ledger.GetExperienceGained(attendeeId2)));
    }

    [Test]
    public async Task ProcessExpProgress_CompleteSurvey_AwardsExpToUser()
    {
        // Arrange
        var userId = "user";
        var ledger = new ExperienceGainedLedger();

        var interestSurveyMock = new Mock<IInterestSurveyRepository>();
        var progressRepositoryMock = new Mock<IProgressRepository>();
        var loggerMock = new Mock<ILogger<ProcessExpProgressHandler>>();

        interestSurveyMock.Setup(x => x.GetNewlyCreatedSurveyUserList()).ReturnsAsync(new List<string>()
        {
            userId
        });

        var expStrategies = new List<IExpStrategy>
        {
            new SurveyCompletedStrategy(interestSurveyMock.Object)
        };

        var testRequest = new ProcessExpProgressRequest(expStrategies);
        var handler = new ProcessExpProgressHandler(loggerMock.Object, progressRepositoryMock.Object, ledger);

        // Act
        await handler.Handle(testRequest, new CancellationToken());

        // Assert
        Assert.That(ledger.GetExperienceGained(userId), Is.GreaterThan(0));
    }

    [Test]
    public async Task ProcessExpProgress_CreateNegativeAndPositiveReview_AwardsExpToUserAndHostCorrectly()
    {
        // Arrange
        var ledger = new ExperienceGainedLedger();
        var dataBuilder = new TestEventObjectBuilder();

        var goodReviewUser = "goodUser1";
        var badReviewUser = "badUser1";
        var goodReviewHost = "goodUser2";
        var badReviewHost = "badUser2";

        var goodEvent = dataBuilder.NewTestEvent(e => { e.Host.UserId = goodReviewHost; });
        var badEvent = dataBuilder.NewTestEvent(e => { e.Host.UserId = badReviewHost; });

        var goodReview = new Review
        {
            Id = 1,
            Rate = 5,
            EventId = goodEvent.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = goodReviewUser
        };
        var badReview = new Review
        {
            Id = 1,
            Rate = 2,
            EventId = goodEvent.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = badReviewUser
        };

        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var progressRepositoryMock = new Mock<IProgressRepository>();
        var eventsRepositoryMock = new Mock<IEventsRepository>();
        var loggerMock = new Mock<ILogger<ProcessExpProgressHandler>>();

        reviewRepositoryMock.Setup(x => x.GetNewReviews()).ReturnsAsync(new List<Review> {badReview, goodReview});
        reviewRepositoryMock.Setup(x => x.GetReviewsCountByUser(badReviewUser)).ReturnsAsync(0);
        reviewRepositoryMock.Setup(x => x.GetReviewsCountByUser(goodReviewUser)).ReturnsAsync(0);
        eventsRepositoryMock.Setup(x => x.GetById(goodEvent.Id)).ReturnsAsync(goodEvent);
        eventsRepositoryMock.Setup(x => x.GetById(badEvent.Id)).ReturnsAsync(badEvent);

        var expStrategies = new List<IExpStrategy>
        {
            new NewReviewsStrategy(reviewRepositoryMock.Object, eventsRepositoryMock.Object,
                progressRepositoryMock.Object)
        };

        var testRequest = new ProcessExpProgressRequest(expStrategies);
        var handler = new ProcessExpProgressHandler(loggerMock.Object, progressRepositoryMock.Object, ledger);

        // Act
        await handler.Handle(testRequest, new CancellationToken());
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(ledger.GetExperienceGained(goodReviewHost),
                Is.GreaterThan(
                    ledger.GetExperienceGained(badReviewHost))); // Good events are rewarded better than bad events
            Assert.That(ledger.GetExperienceGained(goodReviewUser),
                Is.EqualTo(ledger
                    .GetExperienceGained(badReviewUser))); // Good reviews and bad reviews should reward same exp
        });
    }

    [Test]
    public async Task ProcessExpProgress_CreatingReview_PreviousExpShouldGiveBonusExp()
    {
        // Arrange
        var ledger = new ExperienceGainedLedger();
        var dataBuilder = new TestEventObjectBuilder();

        var previousReviewsUser = "previousReviews";
        var firstReviewUser = "badUser1";
        var host = "host";

        var eventToReview = dataBuilder.NewTestEvent(e => { e.Host.UserId = host; });

        var newReview1 = new Review
        {
            Id = 1,
            Rate = 5,
            EventId = eventToReview.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = previousReviewsUser
        };
        var newReview2 = new Review
        {
            Id = 1,
            Rate = 2,
            EventId = eventToReview.Id,
            ReviewDate = DateTimeOffset.UtcNow,
            ReviewerId = firstReviewUser
        };

        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var progressRepositoryMock = new Mock<IProgressRepository>();
        var eventsRepositoryMock = new Mock<IEventsRepository>();
        var loggerMock = new Mock<ILogger<ProcessExpProgressHandler>>();

        reviewRepositoryMock.Setup(x => x.GetNewReviews()).ReturnsAsync(new List<Review> {newReview2, newReview1});
        reviewRepositoryMock.Setup(x => x.GetReviewsCountByUser(firstReviewUser)).ReturnsAsync(0);
        reviewRepositoryMock.Setup(x => x.GetReviewsCountByUser(previousReviewsUser)).ReturnsAsync(1);
        eventsRepositoryMock.Setup(x => x.GetById(eventToReview.Id)).ReturnsAsync(eventToReview);

        var expStrategies = new List<IExpStrategy>
        {
            new NewReviewsStrategy(reviewRepositoryMock.Object, eventsRepositoryMock.Object,
                progressRepositoryMock.Object)
        };

        var testRequest = new ProcessExpProgressRequest(expStrategies);
        var handler = new ProcessExpProgressHandler(loggerMock.Object, progressRepositoryMock.Object, ledger);

        // Act
        await handler.Handle(testRequest, new CancellationToken());

        // Assert
        Assert.That(ledger.GetExperienceGained(previousReviewsUser),
            Is.GreaterThan(ledger.GetExperienceGained(firstReviewUser))); // Bonus for reviewing a lot
    }
    
    [Test]
    public async Task ProcessExpProgress_CompletingSurvey_ProvidesExp()
    {
        // Arrange
        var ledger = new ExperienceGainedLedger();

        var user = "host";

        var interestSurveyMock = new Mock<IInterestSurveyRepository>();
        var progressRepositoryMock = new Mock<IProgressRepository>();
        var loggerMock = new Mock<ILogger<ProcessExpProgressHandler>>();

        interestSurveyMock.Setup(x => x.GetNewlyCreatedSurveyUserList()).ReturnsAsync(new List<string> {user});

        var expStrategies = new List<IExpStrategy>
        {
            new SurveyCompletedStrategy(interestSurveyMock.Object)
        };

        var testRequest = new ProcessExpProgressRequest(expStrategies);
        var handler = new ProcessExpProgressHandler(loggerMock.Object, progressRepositoryMock.Object, ledger);

        // Act
        await handler.Handle(testRequest, new CancellationToken());

        // Assert
        Assert.That(ledger.GetExperienceGained(user),
            Is.GreaterThan(0)); // Bonus for reviewing a lot
    }
}