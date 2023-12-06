namespace EventManagementService.Application.V1.FetchReviewsByUser.Exceptions;

public class InvalidUserIdException : Exception
{
    public InvalidUserIdException(string message): base(message)
    {
    }
}