namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Exceptions;

public class InterestSurveyAlreadyCompletedException: Exception
{
    public InterestSurveyAlreadyCompletedException(string userId, Exception? inner = null): base($"User with id {userId} has already completed their interest survey")
    {
        
    }
}