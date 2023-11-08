namespace EventManagementService.Application.CreateEvents.Exceptions;

public class PubSubPublisherException : Exception
{
    public PubSubPublisherException(string message, Exception e) : base(message, e)
    {
    }
}