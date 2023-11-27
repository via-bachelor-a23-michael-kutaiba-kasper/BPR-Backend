namespace EventManagementService.Application.CreateEvent.Exceptions;

public class CreateEventException : Exception
{
    public CreateEventException(string message, Exception exception) : base(message, exception)
    {
    }
}