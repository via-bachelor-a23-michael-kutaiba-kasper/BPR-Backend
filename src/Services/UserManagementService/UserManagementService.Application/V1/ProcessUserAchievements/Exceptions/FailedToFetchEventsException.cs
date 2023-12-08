namespace UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;

public class FailedToFetchEventsException : Exception
{
    public FailedToFetchEventsException(string message, Exception e) : base(message, e)
    {
    }
}