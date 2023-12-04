using System.Net;
using EventManagementService.API.Controllers.V1.EventControllers.Dtos;
using EventManagementService.API.Controllers.V1.EventControllers.Mappers;
using EventManagementService.Application.V1.CreateEvent;
using EventManagementService.Application.V1.CreateEvent.Exceptions;
using EventManagementService.Application.V1.FetchAllEvents;
using EventManagementService.Application.V1.FetchAllEvents.Model;
using EventManagementService.Application.V1.FetchEventById;
using EventManagementService.Application.V1.ProcessExternalEvents;
using EventManagementService.Infrastructure.Util;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventNotFoundException = EventManagementService.Application.V1.FetchEventById.Exceptions.EventNotFoundException;

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
    public async Task<ActionResult<List<EventDto>>> GetAllEvents([FromQuery] DateTimeOffset? from = null,
        [FromQuery] string hostId = null)
    {
        try
        {
            var events =
                await _mediator.Send(new FetchAllEventsRequest(new Filters {From = from, To = null, HostId = hostId}));
            var eventsAsDtos = events.Select(EventMapper.FromEventToDto);
            return Ok(eventsAsDtos);
        }
        catch (Exception e)
        {
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
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
    public async Task<ActionResult<CreateEventResponseDto>> CreateNewEvent([FromBody] EventDto eventDto)
    {
        try
        {
            var eEvent = await _mediator.Send(new CreateEventRequest(EventMapper.ProcessIncomingEvent(eventDto)));
            var response = new CreateEventResponseDto
            {
                Event = EventMapper.FromEventToDto(eEvent),
                Code = new StatusCode
                {
                    Code = HttpStatusCode.OK,
                    Message = "Event have been successfully created"
                }
            };
            return Ok(response);
        }
        catch (Exception e) when (e is CreateEventException or EventValidationException)
        {
            return StatusCode((int)HttpStatusCode.BadRequest);
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
            return Ok(EventMapper.FromEventToDto(existingEvent));
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