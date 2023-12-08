using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Validation.ValidationRules;

public class MustHaveThreeCategoriesRule: IValidationRule
{
    public string? Check(InterestSurvey survey)
    {
        return survey.Categories.Count != 3 ? "Interest survey must have exactly 3 categories" : null;
    }
}