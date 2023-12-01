namespace EventManagementService.Application.V1.CreateEvent.Exceptions;

public class InsertEventException : Exception
{
    public InsertEventException(string message, Exception exception) : base(message, exception)
    {
    }
}