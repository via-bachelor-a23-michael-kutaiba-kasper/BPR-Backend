namespace EventManagementService.Application.FetchAllPublicEvents.Exceptions;

public class PubSubSubscriberException : Exception
{
    public PubSubSubscriberException(string message, Exception e) : base(message, e)
    {
    }
}