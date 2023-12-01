using System.Net;
using EventManagementService.Application.V1.FetchKeywords;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers.V1.KeywordControllers;

[ApiController]
[Route("api/v1/keywords")]
public class KeywordController : ControllerBase
{
    private readonly IMediator _mediator;

    public KeywordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("allKeywords")]
    public async Task<ActionResult<IReadOnlyCollection<string>>> GetAllKeywords()
    {
        try
        {
            var keywords = await _mediator.Send(new FetchKeywordsRequest());
            return Ok(keywords);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}