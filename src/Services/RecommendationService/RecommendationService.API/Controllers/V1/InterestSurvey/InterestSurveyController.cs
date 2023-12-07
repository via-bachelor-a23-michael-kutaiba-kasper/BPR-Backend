using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RecommendationService.API.Controllers.V1.InterestSurvey;

[ApiController]
[Route("api/v1/interestSurvey")]
public class InterestSurveyController: ControllerBase
{
    private readonly ILogger<InterestSurveyController> _logger;
    private readonly IMediator _mediator;
    public InterestSurveyController(ILogger<InterestSurveyController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
}