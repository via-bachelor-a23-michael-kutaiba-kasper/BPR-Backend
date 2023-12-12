using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Validation.ValidationRules;

public interface IValidationRule
{
    public string? Check(InterestSurvey survey);
}