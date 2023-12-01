namespace EventManagementService.Application.V1.ProcessExternalEvents.Exceptions;

public class PubSubSubscriberException : Exception
{
    public PubSubSubscriberException(string message, Exception e) : base(message, e)
    {
    }
}