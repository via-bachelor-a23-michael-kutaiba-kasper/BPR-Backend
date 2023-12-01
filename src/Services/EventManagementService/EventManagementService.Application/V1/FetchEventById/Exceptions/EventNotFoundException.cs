namespace EventManagementService.Application.V1.FetchEventById.Exceptions;

public class EventNotFoundException: Exception
{
    public EventNotFoundException(int eventId, string? message = null, Exception? inner = null) : base($"Event with id {eventId} does not exist", inner)
    {
    }
}