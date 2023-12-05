namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;

public class InvalidUserIdException : Exception
{
    public InvalidUserIdException(string message): base(message)
    {
    }
}