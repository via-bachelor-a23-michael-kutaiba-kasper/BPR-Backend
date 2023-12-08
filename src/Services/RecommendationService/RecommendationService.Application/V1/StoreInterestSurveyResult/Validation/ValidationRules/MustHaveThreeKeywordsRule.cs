using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Validation.ValidationRules;

public class MustHaveThreeKeywordsRule: IValidationRule
{
    public string? Check(InterestSurvey survey)
    {
        return survey.Keywords.Count != 3 ? "Interest survey must have exactly 3 keywords" : null;
    }
}