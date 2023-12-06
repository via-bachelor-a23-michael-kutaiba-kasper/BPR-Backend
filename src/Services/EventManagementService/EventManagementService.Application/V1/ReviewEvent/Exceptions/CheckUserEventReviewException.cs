namespace EventManagementService.Application.V1.ReviewEvent.Exceptions;

public class CheckUserEventReviewException : Exception
{
    public CheckUserEventReviewException(string message, Exception e) : base(message, e)
    {
    }
}