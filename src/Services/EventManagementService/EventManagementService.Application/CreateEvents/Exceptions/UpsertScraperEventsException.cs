namespace EventManagementService.Application.CreateEvents.Exceptions;

public class UpsertScraperEventsException : Exception
{
    public UpsertScraperEventsException(string message, Exception e) : base(message, e)
    {
    }
}