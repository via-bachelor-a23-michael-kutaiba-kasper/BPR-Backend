using MediatR;
using Microsoft.Extensions.Logging;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult;

public record StoreInterestSurveyRequest(string userId, InterestSurvey survey) : IRequest<InterestSurvey>;

public class StoreInterestSurveyResultHandler: IRequestHandler<StoreInterestSurveyRequest, InterestSurvey>
{
    private readonly ILogger<StoreInterestSurveyResultHandler> _logger;
    public StoreInterestSurveyResultHandler(ILogger<StoreInterestSurveyResultHandler> logger)
    {
        _logger = logger;
    }
    
    public Task<InterestSurvey> Handle(StoreInterestSurveyRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Storing interest survey for user {request.userId}");
        
        throw new NotImplementedException();
    }
}