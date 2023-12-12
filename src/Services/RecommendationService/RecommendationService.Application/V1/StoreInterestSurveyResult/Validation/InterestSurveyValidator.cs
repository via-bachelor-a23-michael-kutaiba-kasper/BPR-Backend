using RecommendationService.Application.V1.StoreInterestSurveyResult.Exceptions;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Validation.ValidationRules;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Validation;

public class InterestSurveyValidator
{
    private readonly IReadOnlyCollection<IValidationRule> _validationRules = new List<IValidationRule>
    {
        new NoDuplicateKeywordsRule(),
        new MustHaveThreeCategoriesRule(),
        new MustHaveThreeKeywordsRule()
    };

    public void Validate(InterestSurvey survey)
    {
        var validationErrors = _validationRules
            .Select(rule => rule.Check(survey))
            .Where(error => error != null)
            .ToList();

        if (validationErrors.Any())
        {
            throw new InterestSurveyValidationError(validationErrors!);
        }
    }
}