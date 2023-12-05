using MediatR;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.V1.GetRecommendations.Engine;
using RecommendationService.Domain;

namespace RecommendationService.Application.V1.GetRecommendations;

public record GetRecommendationsRequest(string UserId, int Limit = 10) : IRequest<Recommendations>;

public class GetRecommendationsHandler : IRequestHandler<GetRecommendationsRequest, Recommendations>
{
    private readonly ILogger<GetRecommendationsHandler> _logger;
    private readonly IRecommendationsEngine _engine;

    public GetRecommendationsHandler(ILogger<GetRecommendationsHandler> logger, IRecommendationsEngine? engine = null)
    {
        _logger = logger;
        _engine = engine ?? new NaiveBayesRecommendationsEngine();
    }

    public Task<Recommendations> Handle(GetRecommendationsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}