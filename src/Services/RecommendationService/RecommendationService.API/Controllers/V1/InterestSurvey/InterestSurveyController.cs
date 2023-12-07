using Microsoft.AspNetCore.Mvc;

namespace RecommendationService.API.Controllers.V1.InterestSurvey;

[ApiController]
[Route("api/v1/interestSurvey")]
public class InterestSurveyController: ControllerBase
{
    private readonly ILogger<InterestSurveyController> _logger;
    public InterestSurveyController(ILogger<InterestSurveyController> logger)
    {
        _logger = logger;
    }
    
    
}