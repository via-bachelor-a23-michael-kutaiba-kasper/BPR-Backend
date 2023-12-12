namespace EventManagementService.Application.V1.JoinEvent.Exceptions;

public class EventHasEndedException: Exception
{
    public EventHasEndedException(int eventId, Exception? inner = null) :
        base($"Unable to join event {eventId}. Event has already ended.")

    {

    }
}