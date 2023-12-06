namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;

public class FailedToFetchUsersException : Exception
{
    public FailedToFetchUsersException(string message, Exception e) : base(message, e)
    {
    }
}