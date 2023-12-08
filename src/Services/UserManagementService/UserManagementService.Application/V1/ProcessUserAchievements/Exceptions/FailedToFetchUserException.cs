namespace UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;

public class FailedToFetchUserException : Exception
{
    public FailedToFetchUserException(string message, Exception e) : base(message, e)
    {
    }
}