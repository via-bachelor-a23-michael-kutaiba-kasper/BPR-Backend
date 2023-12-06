namespace EventManagementService.Application.V1.FetchReviewsByUser.Exceptions;

public class CannotQueryEventReviewsException : Exception
{
    public CannotQueryEventReviewsException(string message, Exception e): base(message, e)
    {
    }
}