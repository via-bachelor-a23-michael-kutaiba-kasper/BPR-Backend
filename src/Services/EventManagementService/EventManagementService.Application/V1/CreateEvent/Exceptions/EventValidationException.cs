namespace EventManagementService.Application.V1.CreateEvent.Exceptions;

public class EventValidationException : Exception
{
    public EventValidationException(string message) : base(message)
    {
    }
}