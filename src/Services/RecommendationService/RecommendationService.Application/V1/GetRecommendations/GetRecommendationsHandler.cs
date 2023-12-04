using MediatR;
using Microsoft.Extensions.Logging;
using RecommendationService.Domain;

namespace RecommendationService.Application.V1.GetRecommendations;

public record GetRecommendationsRequest(string UserId, int Limit = 10) : IRequest<Recommendations>;

public class GetRecommendationsHandler : IRequestHandler<GetRecommendationsRequest, Recommendations>
{
    private readonly ILogger<GetRecommendationsHandler> _logger;

    public GetRecommendationsHandler(ILogger<GetRecommendationsHandler> logger)
    {
        _logger = logger;
    }

    public Task<Recommendations> Handle(GetRecommendationsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}