namespace EventManagementService.Application.V1.FetchReviewsByUser.Exceptions;

public class FetchReviewsByUserException : Exception
{
    public FetchReviewsByUserException(string message, Exception e) : base(message, e)
    {
    }
}