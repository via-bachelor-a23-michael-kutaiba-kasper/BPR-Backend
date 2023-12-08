using RecommendationService.Domain.Events;
using RecommendationService.Domain.Util;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Validation.ValidationRules;

public class NoDuplicateKeywordsRule: IValidationRule
{
    public string? Check(InterestSurvey survey)
    {
        var keywords = survey.Keywords;
        HashSet<Keyword> seenKeywords = new ();
        foreach (var keyword in keywords)
        {
            if (seenKeywords.Contains(keyword))
            {
                return $"Duplicate keyword {keyword.GetDescription()}";
            }

            seenKeywords.Add(keyword);
        }

        return null;
    }
}