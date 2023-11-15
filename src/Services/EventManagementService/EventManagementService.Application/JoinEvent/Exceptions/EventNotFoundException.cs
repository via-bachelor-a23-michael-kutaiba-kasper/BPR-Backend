namespace EventManagementService.Application.JoinEvent.Exceptions;

public class EventNotFoundException: Exception
{
    public EventNotFoundException(int eventId, Exception? inner = null) : base($"No event with id {eventId} exists in the system", inner)
    {
    }
}