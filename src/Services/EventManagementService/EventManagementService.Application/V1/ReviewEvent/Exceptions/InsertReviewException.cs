namespace EventManagementService.Application.V1.ReviewEvent.Exceptions;

public class InsertReviewException : Exception
{
    public InsertReviewException(string message, Exception e) : base(message, e)
    {
    }
}