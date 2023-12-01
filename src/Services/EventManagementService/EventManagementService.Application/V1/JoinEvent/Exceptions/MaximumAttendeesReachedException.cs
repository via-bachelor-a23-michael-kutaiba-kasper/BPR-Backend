namespace EventManagementService.Application.V1.JoinEvent.Exceptions;

public class MaximumAttendeesReachedException : Exception
{
    public MaximumAttendeesReachedException(int eventId, int maxNumberOfAttendees, Exception? inner = null) : base(
        $"The maximum number ({maxNumberOfAttendees}) of attendees for event ${eventId}", inner)

    {
    }
}