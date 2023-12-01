namespace EventManagementService.Application.V1.ProcessExternalEvents.Exceptions;

public class PubSubPublisherException : Exception
{
    public PubSubPublisherException(string message, Exception e) : base(message, e)
    {
    }
}