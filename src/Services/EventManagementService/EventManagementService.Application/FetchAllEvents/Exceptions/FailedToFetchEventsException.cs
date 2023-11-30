namespace EventManagementService.Application.FetchAllEvents.Exceptions;

public class FailedToFetchEventsException: Exception
{
    public FailedToFetchEventsException(Exception? inner = null):base($"Failed to fetch exceptions: {inner?.Message}", inner)
    {
    }
}