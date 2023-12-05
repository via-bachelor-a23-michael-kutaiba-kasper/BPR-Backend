using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecommendationService.Application.V1.GetRecommendations;

namespace RecommendationService.API.Controllers.V1.Recommendation;

[ApiController]
[Route("api/v1/{controller}")]
public class RecommendationsController: ControllerBase
{
    private readonly ILogger<RecommendationsController> _logger;
    private readonly IMediator _mediator;
    public RecommendationsController(Logger<RecommendationsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetRecommendations([FromQuery] string userId, [FromQuery] int limit)
    {
        var recommendations = await _mediator.Send(new GetRecommendationsRequest(userId, limit));
        throw new NotImplementedException();
    }
}