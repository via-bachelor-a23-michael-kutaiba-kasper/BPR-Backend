namespace EventManagementService.Application.V1.ProcessExternalEvents.Exceptions;

public class UpsertEventsException : Exception
{
    public UpsertEventsException(string message, Exception e) : base(message, e)
    {
    }

    public UpsertEventsException()
    {
    }
}