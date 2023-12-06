namespace EventManagementService.Application.V1.ReviewEvent.Exceptions;

public class CannotPublishReviewException : Exception
{
    public CannotPublishReviewException(string message, Exception e) : base(message, e)
    {
    }
}