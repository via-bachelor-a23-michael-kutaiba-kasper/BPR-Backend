namespace RecommendationService.Application.V1.GetInterestSurvey.Exceptions;

public class FailedToFetchException: Exception
{
    public FailedToFetchException(string entity, string source, string? id = null, Exception? inner = null) : base(
        $"Failed to fetch {entity} {(id != null? $"with id {id}" : "")} from {source}", inner)

    {
    }
}