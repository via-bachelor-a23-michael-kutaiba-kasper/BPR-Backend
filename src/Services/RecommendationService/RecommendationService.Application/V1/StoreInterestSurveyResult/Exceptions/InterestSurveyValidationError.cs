namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Exceptions;

public class InterestSurveyValidationError: Exception
{
    public InterestSurveyValidationError(IReadOnlyCollection<string> errors, Exception? inner = null): base($"{string.Join('\n', errors)}", inner)
    {
    }
}