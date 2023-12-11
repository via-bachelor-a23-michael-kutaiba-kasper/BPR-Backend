namespace UserManagementService.Application.V1.FetchUserAchievements.Exceptions;

public class QueryUserAchievementsException : Exception
{
    public QueryUserAchievementsException(string message, Exception exception) : base(message, exception)
    {
    }
}