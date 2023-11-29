using System.Net;
using EventManagementService.API.Controllers.V1.EventControllers.Dtos;
using EventManagementService.Application.CreateEvent;
using EventManagementService.Application.FetchAllEvents;
using EventManagementService.Application.FetchEventById;
using EventManagementService.Application.JoinEvent;
using EventManagementService.Application.JoinEvent.Exceptions;
using EventManagementService.Application.ProcessExternalEvents;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.Util;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventNotFoundException = EventManagementService.Application.FetchEventById.Exceptions.EventNotFoundException;

namespace EventManagementService.API.Controllers.V1.EventControllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventController> _logger;

    public EventController(IMediator mediator, ILogger<EventController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Event>>> GetAllEvents([FromQuery] DateTimeOffset? from=null, [FromQuery] DateTimeOffset? to=null)
    {
        var events = await _mediator.Send(new FetchAllEventsRequest(from, to));

        return Ok(events);
    }

    [HttpGet("externalEvents")]
    public async Task<ActionResult<IReadOnlyCollection<EventDto>>> GetExternalEvents()
    {
        try
        {
            var events = await _mediator.Send(new ProcessExternalEventsRequest());
            var eventsToReturn = events.Select(ev => new EventDto
                {
                    Title = ev.Title,
                    StartDate = ev.StartDate,
                    LastUpdateDate = ev.LastUpdateDate,
                    EndDate = ev.EndDate,
                    CreatedDate = ev.CreatedDate,
                    Host = new UserDto
                    {
                        UserId = ev.Host.UserId,
                        DisplayName = ev.Host.DisplayName,
                        PhotoUrl = ev.Host.PhotoUrl,
                        LastSeenOnline = ev.Host.LastSeenOnline,
                        CreationDate = ev.Host.CreationDate
                    },
                    IsPaid = ev.IsPaid,
                    Description = ev.Description,
                    Category = ev.Category.GetDescription(),
                    Keywords = ev.Keywords.Select(EnumExtensions.GetDescription),
                    AdultsOnly = ev.AdultsOnly,
                    IsPrivate = ev.IsPrivate,
                    MaxNumberOfAttendees = ev.MaxNumberOfAttendees,
                    Location = ev.Location,
                    GeoLocation = new GeoLocationDto
                    {
                        Lat = ev.GeoLocation.Lat,
                        Lng = ev.GeoLocation.Lng
                    },
                    City = ev.City
                })
                .ToList();

            return Ok(eventsToReturn);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("createEvent")]
    public async Task<ActionResult> CreateNewEvent([FromBody] EventDto eventDto)
    {
        try
        {
            await _mediator.Send(new CreateEventRequest(new Event
            {
                Title = eventDto.Title,
                StartDate = eventDto.StartDate,
                LastUpdateDate = eventDto.LastUpdateDate,
                EndDate = eventDto.EndDate,
                CreatedDate = eventDto.CreatedDate,
                Host = new User
                {
                    LastSeenOnline = eventDto.Host.LastSeenOnline,
                    DisplayName = eventDto.Host.DisplayName,
                    PhotoUrl = eventDto.Host.PhotoUrl,
                    UserId = eventDto.Host.UserId,
                },
                IsPaid = eventDto.IsPaid,
                Description = eventDto.Description,
                Category = EnumExtensions.GetEnumValueFromDescription<Category>(eventDto.Category),
                Keywords = eventDto.Keywords.Select(EnumExtensions.GetEnumValueFromDescription<Keyword>),
                AdultsOnly = eventDto.AdultsOnly,
                IsPrivate = eventDto.IsPrivate,
                MaxNumberOfAttendees = eventDto.MaxNumberOfAttendees,
                Location = eventDto.Location,
                GeoLocation = new GeoLocation
                {
                    Lng = eventDto.GeoLocation.Lng,
                    Lat = eventDto.GeoLocation.Lat
                },
                City = eventDto.City
            }));
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<EventDto>> GetEventById([FromRoute] int eventId)
    {
        try
        {
            var existingEvent = await _mediator.Send(new FetchEventByIdRequest(eventId));

            return Ok(new EventDto
            {
                Id = existingEvent.Id,
                Title = existingEvent.Title,
                StartDate = existingEvent.StartDate,
                LastUpdateDate = existingEvent.LastUpdateDate,
                EndDate = existingEvent.EndDate,
                CreatedDate = existingEvent.CreatedDate,
                Host = new UserDto
                {
                    UserId = existingEvent.Host.UserId,
                    LastSeenOnline = existingEvent.Host.LastSeenOnline,
                    DisplayName = existingEvent.Host.DisplayName,
                    PhotoUrl = existingEvent.Host.PhotoUrl,
                    CreationDate = existingEvent.Host.CreationDate,
                },
                IsPaid = existingEvent.IsPaid,
                Description = existingEvent.Description,
                Category = existingEvent.Category.GetDescription(),
                Keywords = existingEvent.Keywords.Select(kw => kw.GetDescription()),
                AdultsOnly = existingEvent.AdultsOnly,
                IsPrivate = existingEvent.IsPrivate,
                MaxNumberOfAttendees = existingEvent.MaxNumberOfAttendees,
                Location = existingEvent.Location,
                GeoLocation = new GeoLocationDto
                {
                    Lat = existingEvent.GeoLocation.Lat,
                    Lng = existingEvent.GeoLocation.Lng
                },
                City = existingEvent.City
            });
        }
        catch (Exception e) when (e is EventNotFoundException)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            _logger.LogError(e.StackTrace);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}