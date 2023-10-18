using EventManagementService.Application.ScraperEvents;
using EventManagementService.Application.ScraperEvents.Repository;
using EventManagementService.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventController : ControllerBase
{
    private ISqlScraperEvents _sqlScraperEvents;

    public EventController(ISqlScraperEvents sqlScraperEvents)
    {
        _sqlScraperEvents = sqlScraperEvents;
    }

    [HttpGet("allEvents")]
    public async Task<ActionResult<List<Event>>> GetEvents()
    {
        return Ok(await _sqlScraperEvents.GetEvents());
    }
}