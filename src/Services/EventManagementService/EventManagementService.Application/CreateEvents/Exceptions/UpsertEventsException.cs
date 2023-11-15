namespace EventManagementService.Application.CreateEvents.Exceptions;

public class UpsertEventsException : Exception
{
    public UpsertEventsException(string message, Exception e) : base(message, e)
    {
    }
}