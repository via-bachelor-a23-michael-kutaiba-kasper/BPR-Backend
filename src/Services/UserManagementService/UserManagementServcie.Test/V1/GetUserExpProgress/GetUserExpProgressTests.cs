using Microsoft.Extensions.Logging;
using Moq;
using UserManagementService.Application.V1.GetUserExpProgres;
using UserManagementService.Application.V1.GetUserExpProgres.Exceptions;
using UserManagementService.Application.V1.GetUserExpProgres.Repository;
using UserManagementService.Domain.Models;

namespace UserManagementServcie.Test.V1.GetUserExpProgress;

[TestFixture]
public class GetUserExpProgressTests
{
    [Test]
    public async Task GetUserExpProgress_UserExistsAndHasExp_ReturnsCorrectProgressWithCorrectLevel()
    {
        // Arrange
        var levels = new List<Level>
        {
            new Level
            {
                MinExp = 0,
                MaxExp = 100,
                Name = "Test 1",
                Value = 1
            },
            new Level
            {
                MinExp = 100,
                MaxExp = 200,
                Name = "Test 1",
                Value = 1
            },
        };

        var user1 = "user1";
        var user2 = "user2";

        var level1Progress = new Progress
        {
            Id = 1,
            Level = new Level(),
            TotalExp = 99
        };
        var level2Progress = new Progress
        {
            Id = 1,
            Level = new Level(),
            TotalExp = 100
        };


        var loggerMock = new Mock<ILogger<GetUserExpProgressHandler>>();
        var levelRepositoryMock = new Mock<ILevelRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var progressRepositoryMock = new Mock<IProgressRepository>();

        userRepositoryMock.Setup(x => x.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        levelRepositoryMock.Setup(x => x.GetAllLevelsAsync()).ReturnsAsync(levels);
        progressRepositoryMock.Setup(x => x.GetUserExpProgressAsync(user1)).ReturnsAsync(level1Progress);
        progressRepositoryMock.Setup(x => x.GetUserExpProgressAsync(user2)).ReturnsAsync(level2Progress);

        var handler = new GetUserExpProgressHandler(loggerMock.Object, progressRepositoryMock.Object,
            userRepositoryMock.Object, levelRepositoryMock.Object);
        var request1 = new GetUserExpProgressRequest(user1);
        var request2 = new GetUserExpProgressRequest(user2);

        // Act
        var user1Progress = await handler.Handle(request1, new CancellationToken());
        var user2Progress = await handler.Handle(request2, new CancellationToken());

        // Assert
        Assert.That(user1Progress.Level, Is.EqualTo(levels[0]));
        Assert.That(user1Progress.TotalExp, Is.EqualTo(99));
        Assert.That(user1Progress.Stage, Is.EqualTo(1));
        Assert.That(user2Progress.Level, Is.EqualTo(levels[1]));
        Assert.That(user2Progress.TotalExp, Is.EqualTo(100));
        Assert.That(user1Progress.Stage, Is.EqualTo(1));
    }

    [Test]
    public void GetUserExpProgress_UserDoesNotExist_ThrowsUserNotFoundException()
    {
        // Arrange

        var loggerMock = new Mock<ILogger<GetUserExpProgressHandler>>();
        var levelRepositoryMock = new Mock<ILevelRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var progressRepositoryMock = new Mock<IProgressRepository>();

        userRepositoryMock.Setup(x => x.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var handler = new GetUserExpProgressHandler(loggerMock.Object, progressRepositoryMock.Object,
            userRepositoryMock.Object, levelRepositoryMock.Object);
        var request = new GetUserExpProgressRequest("test");

        // Act & Assert
        Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(request, new CancellationToken()));
    }
}