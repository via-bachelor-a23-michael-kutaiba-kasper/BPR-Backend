using EventManagementService.Application.ScraperEvents;
using EventManagementService.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private IScraperEvents _scraperEvents;

    public EventController(IScraperEvents scraperEvents)
    {
        _scraperEvents = scraperEvents;
    }

    [HttpGet("allEvents")]
    public async Task<ActionResult<List<Event>>> GetEvents()
    {
        return Ok(await _scraperEvents.GetEvents());
    }
}