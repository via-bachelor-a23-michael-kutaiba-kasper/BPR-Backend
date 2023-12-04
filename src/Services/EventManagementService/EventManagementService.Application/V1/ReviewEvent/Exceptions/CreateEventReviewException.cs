namespace EventManagementService.Application.V1.ReviewEvent.Exceptions;

public class CreateEventReviewException : Exception
{
    public CreateEventReviewException(string message, Exception e) : base(message, e)
    {
    }
}