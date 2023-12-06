using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Microsoft.Extensions.Logging;
using Moq;
using RecommendationService.Application.V1.GetRecommendations;
using RecommendationService.Application.V1.GetRecommendations.Engine;
using RecommendationService.Application.V1.GetRecommendations.Repository;
using RecommendationService.Domain.Events;
using RecommendationService.Infrastructure;
using RecommendationService.Test.Shared;
using RecommendationService.Test.Shared.Builders;

namespace RecommendationService.Test.GetRecommendations;

[TestFixture]
public class GetRecommendationTests
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
    public async Task
        GetRecommendations_UserHasNoReviewsAndHasOnlyAttendedFoodAndDrinkEvents_ReturnsRecommendationsWithFoodAndDrinkEvents()
    {
        // Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<GetRecommendationsHandler>>();
        var eventsRepositoryMock = new Mock<IEventsRepository>();
        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var surveyRepositoryMock = new Mock<ISurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var recommendationsEngine = new FrequencyBasedRecommendationsEngine();

        var userIdToRecommendEventsFor = "Oq8tmHrYV6SeEpWf1olCJNJ1JW99";
        var attendedEvent = dataBuilder.BuildTestEventObject(e =>
        {
            e.Attendees = new List<User> { new User { UserId = userIdToRecommendEventsFor } };
            e.Category = Category.FoodAndDrink;
            e.Keywords = new List<Keyword> { Keyword.Beer, Keyword.BBQ, Keyword.Cocktail };
        });
        var futureEvents = new List<Event>
        {
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Title = "comedy event";
                e.Category = Category.Comedy;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Title = "music event";
                e.Category = Category.Music;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Title = "food and drink event";
                e.Category = Category.FoodAndDrink;
                e.Keywords = new List<Keyword>
                {
                    Keyword.Poetry, Keyword.Coding, Keyword.Blues
                };
            })
        };

        userRepositoryMock.Setup(x => x.GetById(userIdToRecommendEventsFor))
            .ReturnsAsync(new User() { UserId = userIdToRecommendEventsFor });
        eventsRepositoryMock.Setup(x => x.GetAllEvents(It.IsAny<DateTimeOffset>())).ReturnsAsync(futureEvents);
        eventsRepositoryMock.Setup(x => x.GetEventsWhereUserHasAttendedAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Event> { attendedEvent });
        reviewRepositoryMock.Setup(x => x.GetReviewsByUserAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Review>());
        surveyRepositoryMock.Setup(x => x.GetAsync(userIdToRecommendEventsFor)).ReturnsAsync(new InterestSurvey()
        {
            User = new User { UserId = userIdToRecommendEventsFor }, Keywords = new List<Keyword>(),
            Categories = new List<Category>()
        });


        var request = new GetRecommendationsRequest(userIdToRecommendEventsFor, 1);
        var handler = new GetRecommendationsHandler(loggerMock.Object, eventsRepositoryMock.Object,
            reviewRepositoryMock.Object, surveyRepositoryMock.Object, userRepositoryMock.Object, recommendationsEngine);

        // Act
        var recommendations = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(recommendations.Result.Count, Is.EqualTo(1));
        Assert.That(recommendations.Result.First().Event.Category, Is.EqualTo(Category.FoodAndDrink));
    }

    [Test]
    public async Task
        GetRecommendations_UserHasBadReviewOnFoodAndDrinksReviewAndHasOnlyAttendedFoodAndDrinkEvents_ReturnsRecommendationsWhereFoodAndDrinksIsRankedLowest()
    {
        // Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<GetRecommendationsHandler>>();
        var eventsRepositoryMock = new Mock<IEventsRepository>();
        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var surveyRepositoryMock = new Mock<ISurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var recommendationsEngine = new FrequencyBasedRecommendationsEngine();

        var userIdToRecommendEventsFor = "Oq8tmHrYV6SeEpWf1olCJNJ1JW99";
        var attendedEvent = dataBuilder.BuildTestEventObject(e =>
        {
            e.Attendees = new List<User> { new User { UserId = userIdToRecommendEventsFor } };
            e.Category = Category.FoodAndDrink;
            e.Keywords = new List<Keyword> { Keyword.Beer, Keyword.BBQ, Keyword.Cocktail };
        });
        var futureEvents = new List<Event>
        {
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 1;
                e.Title = "comedy event";
                e.Category = Category.Comedy;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 2;
                e.Title = "music event";
                e.Category = Category.Music;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 3;
                e.Title = "food and drink event";
                e.Category = Category.FoodAndDrink;
                e.Keywords = new List<Keyword>
                {
                    Keyword.Poetry, Keyword.Coding, Keyword.Blues
                };
            })
        };

        userRepositoryMock.Setup(x => x.GetById(userIdToRecommendEventsFor))
            .ReturnsAsync(new User() { UserId = userIdToRecommendEventsFor });
        eventsRepositoryMock.Setup(x => x.GetAllEvents(It.IsAny<DateTimeOffset>())).ReturnsAsync(futureEvents);
        eventsRepositoryMock.Setup(x => x.GetEventsWhereUserHasAttendedAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Event> { attendedEvent });
        reviewRepositoryMock.Setup(x => x.GetReviewsByUserAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Review>
            {
                new Review
                {
                    Id = 1,
                    Rate = 0,
                    EventId = 1,
                    ReviewerId = userIdToRecommendEventsFor,
                    ReviewDate = DateTimeOffset.Now
                }
            });
        surveyRepositoryMock.Setup(x => x.GetAsync(userIdToRecommendEventsFor)).ReturnsAsync(new InterestSurvey()
        {
            User = new User { UserId = userIdToRecommendEventsFor }, Keywords = new List<Keyword>(),
            Categories = new List<Category>()
        });


        var request = new GetRecommendationsRequest(userIdToRecommendEventsFor, 3);
        var handler = new GetRecommendationsHandler(loggerMock.Object, eventsRepositoryMock.Object,
            reviewRepositoryMock.Object, surveyRepositoryMock.Object, userRepositoryMock.Object, recommendationsEngine);

        // Act
        var recommendations = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(recommendations.Result.Count, Is.EqualTo(3));
        Assert.That(recommendations.Result.First().Event.Category, Is.Not.EqualTo(Category.FoodAndDrink));
        Assert.That(recommendations.Result.Last().Event.Category, Is.EqualTo(Category.FoodAndDrink));
    }

    [Test]
    public async Task
        GetRecommendations_UserHasNotAttendedAnyEventsButHasInterestSurveyWithFoodAndDrinksSelected_ReturnsRecommendationWithFoodAndDrink()
    {
        // Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<GetRecommendationsHandler>>();
        var eventsRepositoryMock = new Mock<IEventsRepository>();
        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var surveyRepositoryMock = new Mock<ISurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var recommendationsEngine = new FrequencyBasedRecommendationsEngine();

        var userIdToRecommendEventsFor = "Oq8tmHrYV6SeEpWf1olCJNJ1JW99";
        var futureEvents = new List<Event>
        {
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 1;
                e.Title = "comedy event";
                e.Category = Category.Comedy;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 2;
                e.Title = "music event";
                e.Category = Category.Music;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 3;
                e.Title = "food and drink event";
                e.Category = Category.FoodAndDrink;
                e.Keywords = new List<Keyword>
                {
                    Keyword.Poetry, Keyword.Coding, Keyword.Blues
                };
            })
        };

        userRepositoryMock.Setup(x => x.GetById(userIdToRecommendEventsFor))
            .ReturnsAsync(new User() { UserId = userIdToRecommendEventsFor });
        eventsRepositoryMock.Setup(x => x.GetAllEvents(It.IsAny<DateTimeOffset>())).ReturnsAsync(futureEvents);
        eventsRepositoryMock.Setup(x => x.GetEventsWhereUserHasAttendedAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Event> { });
        reviewRepositoryMock.Setup(x => x.GetReviewsByUserAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Review> { });
        surveyRepositoryMock.Setup(x => x.GetAsync(userIdToRecommendEventsFor)).ReturnsAsync(new InterestSurvey()
        {
            User = new User { UserId = userIdToRecommendEventsFor }, Keywords = new List<Keyword>(),
            Categories = new List<Category>() { Category.FoodAndDrink }
        });


        var request = new GetRecommendationsRequest(userIdToRecommendEventsFor, 3);
        var handler = new GetRecommendationsHandler(loggerMock.Object, eventsRepositoryMock.Object,
            reviewRepositoryMock.Object, surveyRepositoryMock.Object, userRepositoryMock.Object, recommendationsEngine);

        // Act
        var recommendations = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(recommendations.Result.Count, Is.EqualTo(3));
        Assert.That(recommendations.Result.First().Event.Category, Is.EqualTo(Category.FoodAndDrink));
    }

    [Test]
    public async Task
        GetRecommendations_UserHasAttendedSingleEvent_ReturnsRecommendationsWithFutureEventsWithSameKeywords()
    {
        // Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<GetRecommendationsHandler>>();
        var eventsRepositoryMock = new Mock<IEventsRepository>();
        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var surveyRepositoryMock = new Mock<ISurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var recommendationsEngine = new FrequencyBasedRecommendationsEngine();

        var userIdToRecommendEventsFor = "Oq8tmHrYV6SeEpWf1olCJNJ1JW99";
        var attendedEvent = dataBuilder.BuildTestEventObject(e =>
        {
            e.Attendees = new List<User> { new User{UserId = userIdToRecommendEventsFor}};
            e.Category = Category.UnAssigned;
            e.Keywords = new List<Keyword> { Keyword.Beer, Keyword.BBQ, Keyword.Cocktail };
        });
        var futureEvents = new List<Event>
        {
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 1;
                e.Title = "comedy event";
                e.Category = Category.Comedy;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 2;
                e.Title = "music event";
                e.Category = Category.Music;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 3;
                e.Title = "food and drink event";
                e.Category = Category.FoodAndDrink;
                e.Keywords = new List<Keyword>
                {
                    Keyword.Beer, Keyword.BBQ, Keyword.Coding
                };
            })
        };

        userRepositoryMock.Setup(x => x.GetById(userIdToRecommendEventsFor))
            .ReturnsAsync(new User() { UserId = userIdToRecommendEventsFor });
        eventsRepositoryMock.Setup(x => x.GetAllEvents(It.IsAny<DateTimeOffset>())).ReturnsAsync(futureEvents);
        eventsRepositoryMock.Setup(x => x.GetEventsWhereUserHasAttendedAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Event> { attendedEvent });
        reviewRepositoryMock.Setup(x => x.GetReviewsByUserAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Review> { });
        surveyRepositoryMock.Setup(x => x.GetAsync(userIdToRecommendEventsFor)).ReturnsAsync(new InterestSurvey()
        {
            User = new User { UserId = userIdToRecommendEventsFor }, Keywords = new List<Keyword>(),
            Categories = new List<Category>()
        });


        var request = new GetRecommendationsRequest(userIdToRecommendEventsFor, 3);
        var handler = new GetRecommendationsHandler(loggerMock.Object, eventsRepositoryMock.Object,
            reviewRepositoryMock.Object, surveyRepositoryMock.Object, userRepositoryMock.Object, recommendationsEngine);

        // Act
        var recommendations = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(recommendations.Result.Count, Is.EqualTo(3));
        Assert.That(recommendations.Result.First().Event.Category, Is.EqualTo(Category.FoodAndDrink));
        Assert.That(recommendations.Result.First().Event.Keywords,
            Has.Some.Matches(new Predicate<Keyword>(kw => kw == Keyword.BBQ)));
        Assert.That(recommendations.Result.First().Event.Keywords,
            Has.Some.Matches(new Predicate<Keyword>(kw => kw == Keyword.Beer)));
    }
    
    [Test]
    public async Task
        GetRecommendations_UserAttendsAllFutureEvents_ReturnsNoRecommendations()
    {
        // Arrange
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<GetRecommendationsHandler>>();
        var eventsRepositoryMock = new Mock<IEventsRepository>();
        var reviewRepositoryMock = new Mock<IReviewRepository>();
        var surveyRepositoryMock = new Mock<ISurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var recommendationsEngine = new FrequencyBasedRecommendationsEngine();

        var userIdToRecommendEventsFor = "Oq8tmHrYV6SeEpWf1olCJNJ1JW99";
        var attendedEvent = dataBuilder.BuildTestEventObject(e =>
        {
            e.Attendees = new List<User> { new User{UserId = userIdToRecommendEventsFor}};
            e.Category = Category.UnAssigned;
            e.Keywords = new List<Keyword> { Keyword.Beer, Keyword.BBQ, Keyword.Cocktail };
        });
        var futureEvents = new List<Event>
        {
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 1;
                e.Title = "comedy event";
                e.Category = Category.Comedy;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
                e.Attendees = new List<User>() { new User{ UserId = userIdToRecommendEventsFor } };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 2;
                e.Title = "music event";
                e.Category = Category.Music;
                e.Keywords = new List<Keyword> { Keyword.Poetry, Keyword.Coding, Keyword.Blues };
                e.Attendees = new List<User>{ new User{ UserId = userIdToRecommendEventsFor } };
            }),
            dataBuilder.BuildTestEventObject(e =>
            {
                e.Id = 3;
                e.Title = "food and drink event";
                e.Category = Category.FoodAndDrink;
                e.Keywords = new List<Keyword>
                {
                    Keyword.Beer, Keyword.BBQ, Keyword.Coding
                };
                e.Attendees = new List<User> { new User { UserId = userIdToRecommendEventsFor } };
            })
        };

        userRepositoryMock.Setup(x => x.GetById(userIdToRecommendEventsFor))
            .ReturnsAsync(new User() { UserId = userIdToRecommendEventsFor });
        eventsRepositoryMock.Setup(x => x.GetAllEvents(It.IsAny<DateTimeOffset>())).ReturnsAsync(futureEvents);
        eventsRepositoryMock.Setup(x => x.GetEventsWhereUserHasAttendedAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Event> { attendedEvent });
        reviewRepositoryMock.Setup(x => x.GetReviewsByUserAsync(userIdToRecommendEventsFor))
            .ReturnsAsync(new List<Review> { });
        surveyRepositoryMock.Setup(x => x.GetAsync(userIdToRecommendEventsFor)).ReturnsAsync(new InterestSurvey()
        {
            User = new User { UserId = userIdToRecommendEventsFor }, Keywords = new List<Keyword>(),
            Categories = new List<Category>()
        });


        var request = new GetRecommendationsRequest(userIdToRecommendEventsFor, 3);
        var handler = new GetRecommendationsHandler(loggerMock.Object, eventsRepositoryMock.Object,
            reviewRepositoryMock.Object, surveyRepositoryMock.Object, userRepositoryMock.Object, recommendationsEngine);

        // Act
        var recommendations = await handler.Handle(request, new CancellationToken());

        // Assert
        Assert.That(recommendations.Result.Count, Is.EqualTo(0));
    }
}