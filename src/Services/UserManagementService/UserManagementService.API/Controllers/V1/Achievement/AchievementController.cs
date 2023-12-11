using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.API.Controllers.V1.Achievement.Dtos;
using UserManagementService.API.Controllers.V1.Achievement.Mappers;
using UserManagementService.Application.V1.FetchUserAchievements;
using UserManagementService.Application.V1.ProcessUserAchievements;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;

namespace UserManagementService.API.Controllers.V1.Achievement;

[ApiController]
[Route("api/v1/progress")]
public class AchievementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AchievementController> _logger;

    public AchievementController(IMediator mediator, ILogger<AchievementController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("achievements/processUserAchievements")]
    public async Task<ActionResult> ProcessUserAchievements()
    {
        try
        {
            await _mediator.Send(new ProcessUserAchievementsRequest());
            return Ok();
        }
        catch (Exception e) when (e is UserNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.BadRequest);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("{userid}/achievements")]
    public async Task<ActionResult<IReadOnlyCollection<AchievementDto>>> GetUserAchievements([FromRoute] string userid)
    {
        try
        {
            var domainAchievements = await _mediator.Send(new FetchUserAchievementsRequest(userid));
            var dto = UserAchievementMapper.FromDomainToDtoMapper(domainAchievements);
            return Ok(dto);
        }
        catch (Exception e) when (e is Application.V1.FetchUserAchievements.Exceptions.UserNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.BadRequest);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}