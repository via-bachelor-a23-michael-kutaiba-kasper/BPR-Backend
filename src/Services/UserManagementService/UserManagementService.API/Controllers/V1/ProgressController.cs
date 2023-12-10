using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Application.V1.ProcessExpProgress;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

namespace UserManagementService.API.Controllers.V1;

[ApiController]
[Route("api/v1/progress")]
public class ProgressController : ControllerBase
{
    private readonly ILogger<ProgressController> _logger;
    private readonly IEnumerable<IExpStrategy> _expStrategies;
    private readonly IMediator _mediator;

    public ProgressController(IMediator mediator, ILogger<ProgressController> logger,
        IEnumerable<IExpStrategy> expStrategies)
    {
        _logger = logger;
        _expStrategies = expStrategies;
        _mediator = mediator;
    }

    [HttpPost("exp")]
    public async Task<ActionResult> ProcessExpProgress()
    {
        try
        {
            await _mediator.Send(new ProcessExpProgressRequest(_expStrategies));
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to process EXP progress");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }
}