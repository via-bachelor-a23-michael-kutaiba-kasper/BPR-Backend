using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecommendationService.API.Controllers.V1.Recommendation.Mappers;
using RecommendationService.Application.V1.GetRecommendations;

namespace RecommendationService.API.Controllers.V1.Recommendation;

[ApiController]
[Route("api/v1/recommendations")]
public class RecommendationsController: ControllerBase
{
    private readonly ILogger<RecommendationsController> _logger;
    private readonly IMediator _mediator;
    public RecommendationsController(ILogger<RecommendationsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetRecommendations([FromQuery] string userId, [FromQuery] int limit)
    {
        var recommendations = await _mediator.Send(new GetRecommendationsRequest(userId, limit));
        return Ok(RecommendationsMapper.FromDomainToRecommendationsDto(recommendations));
    }
}