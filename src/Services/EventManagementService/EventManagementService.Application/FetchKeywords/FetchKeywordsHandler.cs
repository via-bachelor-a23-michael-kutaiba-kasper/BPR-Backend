using EventManagementService.Application.FetchKeywords.Exceptions;
using EventManagementService.Application.FetchKeywords.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.FetchKeywords;

public record FetchKeywordsRequest() : IRequest<IReadOnlyCollection<string>>;

public class FetchKeywordsHandler : IRequestHandler<FetchKeywordsRequest, IReadOnlyCollection<string>>
{
    private readonly ILogger<FetchKeywordsHandler> _logger;
    private readonly ISqlFetchKeywords _sqlFetchKeywords;

    public FetchKeywordsHandler
    (
        ILogger<FetchKeywordsHandler> logger,
        ISqlFetchKeywords sqlFetchKeywords
    )
    {
        _logger = logger;
        _sqlFetchKeywords = sqlFetchKeywords;
    }

    public async Task<IReadOnlyCollection<string>> Handle
    (
        FetchKeywordsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var keywords = await _sqlFetchKeywords.FetchKeywords();
            _logger.LogInformation($"{keywords.Count} keywords successfully fetched at: {DateTimeOffset.UtcNow}");
            return keywords;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error while fetching categories");
            throw new FetchKeywordsException(
                $"Something went wrong while fetching categories at : {DateTimeOffset.UtcNow}", e);
        }
    }
}