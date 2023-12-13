namespace EventManagementService.Infrastructure.Exceptions;

public class SnapshotDoesNotExistException : Exception
{
    public SnapshotDoesNotExistException(string message) : base(message)
    {
    }
}