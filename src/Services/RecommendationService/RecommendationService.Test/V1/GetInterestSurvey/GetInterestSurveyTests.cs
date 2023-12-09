using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Microsoft.Extensions.Logging;
using Moq;
using RecommendationService.Application.V1.GetInterestSurvey;
using RecommendationService.Application.V1.GetInterestSurvey.Exceptions;
using RecommendationService.Application.V1.GetInterestSurvey.Repositories;
using RecommendationService.Domain.Events;

namespace RecommendationService.Test.V1.GetInterestSurvey;

public class GetInterestSurveyTests
{
    
    [Test]
    public void GetInterestSurvey_UserDoesNotExist_ThrowsUserNotFoundException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<GetInterestSurveyHandler>>();
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
        var request = new GetInterestSurveyRequest("test");
        var handler = new GetInterestSurveyHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act & Assert 
        Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(request, new CancellationToken()));
    }
    
    [Test]
    public async Task GetInterestSurvey_UserExistsButHasNotCompletedInterestSurvey_ReturnsNull()
    {
        // Arrange
        var testUser = new User
        {
            UserId = "test"
        };
        
        var loggerMock = new Mock<ILogger<GetInterestSurveyHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId)).ReturnsAsync(testUser);
        surveyRepositoryMock.Setup(x => x.GetInterestSurvey(testUser.UserId)).ReturnsAsync((InterestSurvey) null!);

        var request = new GetInterestSurveyRequest("test");
        var handler = new GetInterestSurveyHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act 
        var survey = await handler.Handle(request, new CancellationToken());
        
        // Assert
        Assert.That(survey, Is.Null);
    }
    [Test]
    public async Task GetInterestSurvey_UserExistsAndHasCompletedSurvey_ReturnsSurvey()
    {
        // Arrange
        var testUser = new User
        {
            UserId = "test"
        };
        var testSurvey = new InterestSurvey
        {
            User = testUser,
            Categories = new List<Category>(),
            Keywords = new List<Keyword>()
        };
        
        var loggerMock = new Mock<ILogger<GetInterestSurveyHandler>>();
        var surveyRepositoryMock= new Mock<IInterestSurveyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId)).ReturnsAsync(testUser);
        surveyRepositoryMock.Setup(x => x.GetInterestSurvey(testUser.UserId)).ReturnsAsync(testSurvey);

        var request = new GetInterestSurveyRequest(testUser.UserId);
        var handler = new GetInterestSurveyHandler(loggerMock.Object, surveyRepositoryMock.Object,
            userRepositoryMock.Object);
        
        // Act 
        var survey = await handler.Handle(request, new CancellationToken());
        
        // Assert
        Assert.That(survey, Is.EqualTo(testSurvey));
    }
}