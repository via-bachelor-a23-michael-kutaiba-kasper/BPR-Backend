namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;

public class CannotQueryFinishedParticipatedEventsUser : Exception
{
    public CannotQueryFinishedParticipatedEventsUser(string message, Exception e): base(message, e)
    {
    }
}