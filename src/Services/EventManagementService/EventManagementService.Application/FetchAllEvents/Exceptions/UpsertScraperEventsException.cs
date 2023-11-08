namespace EventManagementService.Application.FetchAllEvents.Exceptions;

public class UpsertScraperEventsException : Exception
{
    public UpsertScraperEventsException(string message, Exception e) : base(message, e)
    {
    }
}