using System.Net;
using EventManagementService.API.Controllers.V1.EventControllers.Dtos;
using EventManagementService.Application.CreateEvent;
using EventManagementService.Application.FetchAllEvents;
using EventManagementService.Application.JoinEvent;
using EventManagementService.Application.JoinEvent.Exceptions;
using EventManagementService.Application.ProcessExternalEvents;
using EventManagementService.Domain.Models.Events;
using EventManagementService.Infrastructure.Util;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers.V1.EventControllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventController> _logger;

    public EventController
    (
        IMediator mediator,
        ILogger<EventController> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("allEvents")]
    public async Task<ActionResult<List<Event>>> GetAllEvents()
    {
        // TODO: get these from appsetiing.json
        /*
        TopicName topicName = new TopicName("bachelorshenanigans", "vibeverse_events_scraped");
        SubscriptionName subscriptionName = new SubscriptionName("bachelorshenanigans", "eventmanagement");
        */

        var events = await _mediator.Send(new AllEventsRequest());

        return Ok(events);
    }

    [HttpPost("{eventId}/attendees")]
    public async Task<ActionResult> JoinEvent([FromRoute] int eventId, [FromBody] JoinEventDto joinEventDto)
    {
        try
        {
            await _mediator.Send(new JoinEventRequest(joinEventDto.UserId, eventId));
            return Ok();
        }
        catch (Exception e) when (e is UserNotFoundException or EventNotFoundException)
        {
            return NotFound(e.Message);
        }
        catch (AlreadyJoinedException e)
        {
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
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
                    HostId = ev.HostId,
                    IsPaid = ev.IsPaid,
                    Description = ev.Description,
                    Category = ev.Category.GetDescription(),
                    Keywords = ev.Keywords.Select(EnumExtensions.GetDescription),
                    AdultsOnly = ev.AdultsOnly,
                    IsPrivate = ev.IsPrivate,
                    MaxNumberOfAttendees = ev.MaxNumberOfAttendees,
                    Location = new LocationDto
                    {
                        Country = ev.Location.Country,
                        StreetName = ev.Location.StreetName,
                        StreetNumber = ev.Location.StreetNumber,
                        HouseNumber = ev.Location.HouseNumber,
                        PostalCode = ev.Location.PostalCode,
                        City = ev.Location.City,
                        SubPremise = ev.Location.SubPremise,
                        Lat = ev.Location.GeoLocation.Lat,
                        Lng = ev.Location.GeoLocation.Lng
                    }
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
                HostId = eventDto.HostId,
                IsPaid = eventDto.IsPaid,
                Description = eventDto.Description,
                Category = EnumExtensions.GetEnumValueFromDescription<Category>(eventDto.Category),
                Keywords = eventDto.Keywords.Select(EnumExtensions.GetEnumValueFromDescription<Keyword>),
                AdultsOnly = eventDto.AdultsOnly,
                IsPrivate = eventDto.IsPrivate,
                MaxNumberOfAttendees = eventDto.MaxNumberOfAttendees,
                Location = new Location
                {
                    Country = eventDto.Location.Country,
                    StreetName = eventDto.Location.StreetName,
                    StreetNumber = eventDto.Location.StreetNumber,
                    HouseNumber = eventDto.Location.HouseNumber,
                    PostalCode = eventDto.Location.PostalCode,
                    City = eventDto.Location.City,
                    SubPremise = eventDto.Location.SubPremise,
                    GeoLocation = new GeoLocation
                    {
                        Lat = eventDto.Location.Lat,
                        Lng = eventDto.Location.Lng
                    }
                }
            }));
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}