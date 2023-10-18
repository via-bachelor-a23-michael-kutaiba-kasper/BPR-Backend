using EventManagementService.Application.ScraperEvents;
using EventManagementService.Application.ScraperEvents.Repository;
using EventManagementService.Domain.Models;
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

    [HttpGet("allEvents")]
    public async Task<ActionResult<List<Event>>> GetEvents()
    {
        TopicName topicName = new TopicName("pubsubtest", "test");
        SubscriptionName subscriptionName = new SubscriptionName("pubsubtest", "testsub");

        var events = await _mediator.Send(new ScraperEventsRequest(topicName, subscriptionName));
        
        return Ok(events);
    }
}