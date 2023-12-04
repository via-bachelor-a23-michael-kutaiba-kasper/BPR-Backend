using Microsoft.AspNetCore.Mvc;

namespace RecommendationService.API.Controllers.V1.Recommendation;

[ApiController]
[Route("api/v1/{controller}")]
public class RecommendationsController: ControllerBase
{
    private readonly ILogger<RecommendationsController> _logger;
    public RecommendationsController(Logger<RecommendationsController> logger)
    {
        _logger = logger;
    }
}