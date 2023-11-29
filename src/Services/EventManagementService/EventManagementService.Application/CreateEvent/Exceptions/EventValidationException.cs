namespace EventManagementService.Application.CreateEvent.Exceptions;

public class EventValidationException : Exception
{
    public EventValidationException(string message) : base(message)
    {
    }
}