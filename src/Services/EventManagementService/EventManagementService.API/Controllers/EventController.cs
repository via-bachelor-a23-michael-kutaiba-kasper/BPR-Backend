using EventManagementService.Application.FetchAllPublicEvents;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Google.Cloud.PubSub.V1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers;

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

    [HttpGet("allPublicEvents")]
    public async Task<ActionResult<List<Event>>> GetPublicEvents()
    {
        // TODO: get these from appsetiing.json
        TopicName topicName = new TopicName("bachelorshenanigans", "vibeverse_events_scraped");
        SubscriptionName subscriptionName = new SubscriptionName("bachelorshenanigans", "eventmanagement");

        var events = await _mediator.Send(new ScraperEventsRequest(topicName, subscriptionName));
        
        return Ok(events);
    }
}