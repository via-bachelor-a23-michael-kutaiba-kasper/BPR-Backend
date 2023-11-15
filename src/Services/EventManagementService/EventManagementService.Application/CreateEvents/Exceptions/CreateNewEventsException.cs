namespace EventManagementService.Application.CreateEvents.Exceptions;

public class CreateNewEventsException : Exception
{
    public CreateNewEventsException(string message, Exception e): base(message, e)
    {
    }
}