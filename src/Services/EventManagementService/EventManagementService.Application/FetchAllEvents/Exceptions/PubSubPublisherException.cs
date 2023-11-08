namespace EventManagementService.Application.FetchAllEvents.Exceptions;

public class PubSubPublisherException : Exception
{
    public PubSubPublisherException(string message, Exception e) : base(message, e)
    {
    }
}