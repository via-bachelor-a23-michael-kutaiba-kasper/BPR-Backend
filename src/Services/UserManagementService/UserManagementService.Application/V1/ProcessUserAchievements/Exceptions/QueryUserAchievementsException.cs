namespace UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;

public class QueryUserAchievementsException : Exception
{
    public QueryUserAchievementsException(string message, Exception e) : base(message, e)
    {
    }
}