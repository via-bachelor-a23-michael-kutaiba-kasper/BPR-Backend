namespace EventManagementService.Application.ProcessExternalEvents.Exceptions;

public class CreateNewEventsException : Exception
{
    public CreateNewEventsException(string message, Exception e): base(message, e)
    {
    }
}