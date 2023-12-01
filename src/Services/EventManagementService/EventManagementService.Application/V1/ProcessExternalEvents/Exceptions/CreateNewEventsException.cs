namespace EventManagementService.Application.V1.ProcessExternalEvents.Exceptions;

public class CreateNewEventsException : Exception
{
    public CreateNewEventsException(string message, Exception e): base(message, e)
    {
    }

    public CreateNewEventsException()
    {
    }
}