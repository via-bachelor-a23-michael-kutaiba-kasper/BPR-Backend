namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;

public class FetchFinishedParticipatedInEventsByUserException : Exception
{
    public FetchFinishedParticipatedInEventsByUserException(string message, Exception e) : base(message, e)
    {
    }
}