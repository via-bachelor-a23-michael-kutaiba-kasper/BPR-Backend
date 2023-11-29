using EventManagementService.Application.CreateEvent.Exceptions;
using EventManagementService.Application.CreateEvent.Repository;
using EventManagementService.Application.CreateEvent.Validators;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.Util;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.CreateEvent;

public record CreateEventRequest(Event Event) : IRequest;

public class CreateEventHandler : IRequestHandler<CreateEventRequest>
{
    private readonly ISqlCreateEvent _sqlCreateEvent;
    private readonly ILogger<CreateEventHandler> _logger;

    public CreateEventHandler(ISqlCreateEvent sqlCreateEvent, ILogger<CreateEventHandler> logger)
    {
        _sqlCreateEvent = sqlCreateEvent;
        _logger = logger;
    }

    public async Task Handle(CreateEventRequest request, CancellationToken cancellationToken)
    {
        try
        {
            
            var mappedEvent = MapEvent(request);
            
            EventValidator.ValidateEvents(mappedEvent);
            
            await _sqlCreateEvent.InsertEvent(mappedEvent);
            
            _logger.LogInformation($"Event has been successfully created at: {DateTimeOffset.UtcNow}");
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Cannot create new event at: {DateTimeOffset.UtcNow}");
            throw new CreateEventException(
                $"Something went wrong while creating a new event at: {DateTimeOffset.UtcNow}", e);
        }
    }

    private static Event MapEvent(CreateEventRequest request)
    {
        return new Event
        {
            Title = request.Event.Title,
            StartDate = request.Event.StartDate,
            EndDate = request.Event.EndDate,
            CreatedDate = request.Event.CreatedDate,
            LastUpdateDate = request.Event.LastUpdateDate,
            IsPrivate = request.Event.IsPaid,
            AdultsOnly = request.Event.AdultsOnly,
            IsPaid = request.Event.IsPaid,
            Host = new User
            {
                UserId = request.Event.Host.UserId,
                DisplayName = request.Event.Host.DisplayName,
                PhotoUrl = request.Event.Host.PhotoUrl,
                LastSeenOnline = request.Event.Host.LastSeenOnline,
                CreationDate = request.Event.Host.CreationDate
            },
            MaxNumberOfAttendees = request.Event.MaxNumberOfAttendees,
            Url = request.Event.Url,
            Description = request.Event.Description,
            Location = request.Event.Location,
            City = request.Event.City,
            GeoLocation = new GeoLocation
            {
                Lat = request.Event.GeoLocation.Lat,
                Lng = request.Event.GeoLocation.Lng
            },
            AccessCode =
                UniqueEventAccessCodeGenerator.GenerateUniqueString(request.Event.Title, request.Event.CreatedDate),
            Category = request.Event.Category,
            Keywords = request.Event.Keywords,
            Images = request.Event.Images,
            Attendees = request.Event.Attendees
        };
    }
}