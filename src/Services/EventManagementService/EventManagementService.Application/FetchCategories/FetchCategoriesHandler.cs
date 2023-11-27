using EventManagementService.Application.FetchCategories.Exceptions;
using EventManagementService.Application.FetchCategories.Repository;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.FetchCategories;

public record FetchCategoriesRequest() : IRequest<IReadOnlyCollection<string>>;

public class FetchCategoriesHandler : IRequestHandler<FetchCategoriesRequest, IReadOnlyCollection<string>>
{
    private readonly ILogger<FetchCategoriesHandler> _logger;
    private readonly ISqlFetchCategories _sqlFetchCategories;

    public FetchCategoriesHandler
    (
        ILogger<FetchCategoriesHandler> logger,
        ISqlFetchCategories sqlFetchCategories
    )
    {
        _logger = logger;
        _sqlFetchCategories = sqlFetchCategories;
    }

    public async Task<IReadOnlyCollection<string>> Handle
    (
        FetchCategoriesRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var categories = await _sqlFetchCategories.GetCategories();
            _logger.LogInformation($"{categories.Count} categories successfully fetched at: {DateTimeOffset.UtcNow}");
            return categories;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error while fetching categories");
            throw new FetchCategoriesException(
                $"Something went wrong while fetching categories at : {DateTimeOffset.UtcNow}", e);
        }
    }
}