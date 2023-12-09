using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Microsoft.Extensions.Logging;
using Moq;
using RecommendationService.Application.V1.StoreInterestSurveyResult;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Exceptions;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Repositories;
using RecommendationService.Domain.Events;

namespace RecommendationService.Test.V1.StoreInterestSurveyResult;

[TestFixture]
public class StoreInterestSurveyResultTests
{

    [Test]
    public void StoreInterestSurveyResult_UserDoesNotExist_ThrowsUserNotFoundException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<StoreInterestSurveyResultHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User) null!);

        var testSurvey = new InterestSurvey
        {
            User = new User
            {
                UserId = "test"
            },
            Categories = new List<Category>(),
            Keywords = new List<Keyword>()
        };
        var request = new StoreInterestSurveyRequest("test", testSurvey);
        var handler = new StoreInterestSurveyResultHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act & Assert 
        Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(request, new CancellationToken()));
    }
    
    [Test]
    public void StoreInterestSurveyResult_UserHasAlreadyCompletedSurvey_ThrowsInterestSurveyAlreadyCompletedException()
    {
        // Arrange
        var testSurvey = new InterestSurvey
        {
            User = new User
            {
                UserId = "test"
            },
            Categories = new List<Category>(),
            Keywords = new List<Keyword>()
        };
        var testUser = new User
        {
            UserId = "test"
        };
        
        var loggerMock = new Mock<ILogger<StoreInterestSurveyResultHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId)).ReturnsAsync(testUser);
        surveyRepositoryMock.Setup(x => x.GetInterestSurvey(testUser.UserId)).ReturnsAsync(testSurvey);

        var request = new StoreInterestSurveyRequest(testUser.UserId, testSurvey);
        var handler = new StoreInterestSurveyResultHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act & Assert 
        Assert.ThrowsAsync<InterestSurveyAlreadyCompletedException>(() => handler.Handle(request, new CancellationToken()));
    }
    [Test]
    public async Task StoreInterestSurveyResult_UserExistsAndHasNotCompletedSurvey_ReturnsStoredSurvey()
    {
        // Arrange
        var testSurvey = new InterestSurvey
        {
            User = new User
            {
                UserId = "test"
            },
            Categories = new List<Category> {Category.Games, Category.Concerts, Category.Comedy},
            Keywords = new List<Keyword> {Keyword.Beer, Keyword.Basketball, Keyword.Cycling}
        };
        var testUser = new User
        {
            UserId = "test"
        };
        
        var loggerMock = new Mock<ILogger<StoreInterestSurveyResultHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId)).ReturnsAsync(testUser);
        surveyRepositoryMock.Setup(x => x.GetInterestSurvey(testUser.UserId)).ReturnsAsync((InterestSurvey) null!);
        surveyRepositoryMock.Setup(x => x.StoreInterestSurvey(testUser.UserId, testSurvey)).ReturnsAsync(testSurvey);

        var request = new StoreInterestSurveyRequest(testUser.UserId, testSurvey);
        var handler = new StoreInterestSurveyResultHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act 
        var storedSurvey = await handler.Handle(request, new CancellationToken());
        
        // Assert
        Assert.That(storedSurvey.User.UserId, Is.EqualTo("test"));
        Assert.That(storedSurvey.Keywords.Count, Is.EqualTo(3));
        Assert.That(storedSurvey.Categories.Count, Is.EqualTo(3));
    }
    
    [TestCase()]
    [TestCase(Category.Comedy)]
    [TestCase(Category.Comedy, Category.Concerts)]
    [TestCase(Category.Comedy, Category.Concerts, Category.Comedy, Category.Drinks)]
    public async Task StoreInterestSurveyResult_Not3Categories_ShouldThrowValidationError(params Category[] categories)
    {
        // Arrange
        var testSurvey = new InterestSurvey
        {
            User = new User
            {
                UserId = "test"
            },
            Categories = categories.ToList(),
            Keywords = new List<Keyword> {Keyword.Beer, Keyword.Basketball, Keyword.Cycling}
        };
        var testUser = new User
        {
            UserId = "test"
        };
        
        var loggerMock = new Mock<ILogger<StoreInterestSurveyResultHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId)).ReturnsAsync(testUser);
        surveyRepositoryMock.Setup(x => x.GetInterestSurvey(testUser.UserId)).ReturnsAsync((InterestSurvey) null!);
        surveyRepositoryMock.Setup(x => x.StoreInterestSurvey(testUser.UserId, testSurvey)).ReturnsAsync(testSurvey);

        var request = new StoreInterestSurveyRequest(testUser.UserId, testSurvey);
        var handler = new StoreInterestSurveyResultHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act 
        Assert.ThrowsAsync<InterestSurveyValidationError>(() => handler.Handle(request, new CancellationToken()));
    }
    
    [TestCase()]
    [TestCase(Keyword.Beer)]
    [TestCase(Keyword.Beer, Keyword.Basketball)]
    [TestCase(Keyword.Beer, Keyword.Basketball, Keyword.Cycling, Keyword.Blues)]
    public void StoreInterestSurveyResult_Not3Keywords_ShouldThrowValidationError(params Keyword[] keywords)
    {
        // Arrange
        var testSurvey = new InterestSurvey
        {
            User = new User
            {
                UserId = "test"
            },
            Categories = new List<Category> {Category.Comedy, Category.Concerts, Category.PerformingArts},
            Keywords = keywords.ToList()
        };
        var testUser = new User
        {
            UserId = "test"
        };
        
        var loggerMock = new Mock<ILogger<StoreInterestSurveyResultHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId)).ReturnsAsync(testUser);
        surveyRepositoryMock.Setup(x => x.GetInterestSurvey(testUser.UserId)).ReturnsAsync((InterestSurvey) null!);
        surveyRepositoryMock.Setup(x => x.StoreInterestSurvey(testUser.UserId, testSurvey)).ReturnsAsync(testSurvey);

        var request = new StoreInterestSurveyRequest(testUser.UserId, testSurvey);
        var handler = new StoreInterestSurveyResultHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act 
        Assert.ThrowsAsync<InterestSurveyValidationError>(() => handler.Handle(request, new CancellationToken()));
    }
    
    [Test]
    public void StoreInterestSurveyResult_DuplicateKeywords_ShouldThrowValidationError()
    {
        // Arrange
        var testSurvey = new InterestSurvey
        {
            User = new User
            {
                UserId = "test"
            },
            Categories = new List<Category> {Category.Comedy, Category.Concerts, Category.PerformingArts},
            Keywords = new List<Keyword>() {Keyword.Basketball, Keyword.Basketball, Keyword.Beer}
        };
        var testUser = new User
        {
            UserId = "test"
        };
        
        var loggerMock = new Mock<ILogger<StoreInterestSurveyResultHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId)).ReturnsAsync(testUser);
        surveyRepositoryMock.Setup(x => x.GetInterestSurvey(testUser.UserId)).ReturnsAsync((InterestSurvey) null!);
        surveyRepositoryMock.Setup(x => x.StoreInterestSurvey(testUser.UserId, testSurvey)).ReturnsAsync(testSurvey);

        var request = new StoreInterestSurveyRequest(testUser.UserId, testSurvey);
        var handler = new StoreInterestSurveyResultHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act 
        Assert.ThrowsAsync<InterestSurveyValidationError>(() => handler.Handle(request, new CancellationToken()));
    }
}