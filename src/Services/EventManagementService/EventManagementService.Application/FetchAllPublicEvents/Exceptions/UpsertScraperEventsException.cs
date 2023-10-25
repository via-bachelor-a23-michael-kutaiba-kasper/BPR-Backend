namespace EventManagementService.Application.FetchAllPublicEvents.Exceptions;

public class UpsertScraperEventsException : Exception
{
    public UpsertScraperEventsException(string message, Exception e) : base(message, e)
    {
    }
}