using System.Runtime.CompilerServices;
using EventManagementService.Application.JoinEvent;
using EventManagementService.Application.JoinEvent.Repositories;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure;
using EventManagementService.Test.JoinEvent.Utils;
using EventManagementService.Test.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventManagementService.Test.JoinEvent;

[TestFixture]
public class JoinEventIntegration
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
    public async Task JoinEvent_AddsNewAttendee()
    {
        var dataBuilder = new DataBuilder(_connectionStringManager);
        var loggerMock = new Mock<ILogger<JoinEventHandler>>();
        var invitationRepositoryMock = new Mock<IInvitationRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var eventRepository = new EventRepository(_connectionStringManager);

        var existingEvent = dataBuilder.NewTestEvent((e) => e.Attendees = new List<string>());
        dataBuilder
            .CreateLocations(new List<Location>() { existingEvent.Location });
        existingEvent.Location = dataBuilder.LocationsSet[0];
        dataBuilder.CreateEvents(new List<Event>() { existingEvent });
        existingEvent = dataBuilder.EventSet[0];
        
        var existingUserId = "Oq8tmUrDV6SeEpWf1olCJNJ1JW93";
        
        userRepositoryMock.Setup(x => x.UserExistsAsync(existingUserId)).ReturnsAsync(true);
        invitationRepositoryMock.Setup(x => x.GetInvitationsAsync(existingEvent.Id))
            .ReturnsAsync(new List<Invitation>());
            
        var joinEventRequest = new JoinEventRequest(existingUserId, existingEvent.Id);
        var handler = new JoinEventHandler(loggerMock.Object, eventRepository, invitationRepositoryMock.Object, userRepositoryMock.Object);

        await handler.Handle(joinEventRequest, new CancellationToken());

        var updatedEvent = await eventRepository.GetByIdAsync(existingEvent.Id);
        Assert.IsNotNull(updatedEvent);
        Assert.That(updatedEvent.Attendees.Count(), Is.EqualTo(1));
        Assert.That(updatedEvent.Attendees.First(), Is.EqualTo(existingUserId));
    }
}