namespace EventManagementService.Application.V1.ReviewEvent.Exceptions;

public class ReviewAlreadyExistException : Exception
{
    public ReviewAlreadyExistException(string message) : base(message)
    {
    }
}