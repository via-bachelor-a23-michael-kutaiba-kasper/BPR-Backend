namespace UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;

public class ProcessUserAchievementException : Exception
{
    public ProcessUserAchievementException(string message, Exception e) : base(message, e)
    {
    }
}