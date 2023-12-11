using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.API.Controllers.V1.ExpProgress.Dtos;
using UserManagementService.Application.V1.GetUserExpProgres;
using UserManagementService.Application.V1.GetUserExpProgres.Exceptions;
using UserManagementService.Application.V1.ProcessExpProgress;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

namespace UserManagementService.API.Controllers.V1.ExpProgress;

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

    [HttpGet("{userId}/exp")]
    public async Task<ActionResult<ReadUserExpDto>> GetUserExp([FromRoute] string userId)
    {
        try
        {
            var progress = await _mediator.Send(new GetUserExpProgressRequest(userId));
            var dto = new ReadUserExpDto
            {
                TotalExp = progress.TotalExp,
                Level = new ReadLevelDto
                {
                    Value = progress.Level.Value,
                    MinExp = progress.Level.MinExp,
                    MaxExp = progress.Level.MaxExp,
                    Name = progress.Level.Name
                },
                ExpProgressHistory = progress.ExpProgressHistory.Select(entry => new ReadExpProgressEntryDto()
                {
                    ExpGained = entry.ExpGained,
                    Timestamp = entry.Timestamp
                }).ToList()
            };

            return Ok(dto);
        }
        catch (Exception e) when (e is UserNotFoundException)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to EXP progress for user {userId}");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }

}