namespace UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;

public class InsertUserAchievementException : Exception
{
    public InsertUserAchievementException(string message, Exception e) : base(message, e)
    {
    }
}