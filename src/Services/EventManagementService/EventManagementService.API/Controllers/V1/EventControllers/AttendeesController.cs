using System.Net;
using EventManagementService.API.Controllers.V1.EventControllers.Dtos;
using EventManagementService.Application.V1.JoinEvent;
using EventManagementService.Application.V1.JoinEvent.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers.V1.EventControllers;

[ApiController]
[Route("api/v1/attendees")]
public class AttendeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AttendeesController> _logger;

    public AttendeesController
    (
        IMediator mediator,
        ILogger<AttendeesController> logger
    )
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> JoinEvent([FromBody] JoinEventDto joinEventDto)
    {
        try
        {
            await _mediator.Send(new JoinEventRequest(joinEventDto.UserId, joinEventDto.EventId));
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
        catch (EventHasEndedException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            _logger.LogError(e.StackTrace);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}