namespace UserManagementService.Application.V1.FetchAllAchievements.Exceptions;

public class QueryAllAchievementsException : Exception
{
    public QueryAllAchievementsException(string message, Exception e) : base(message, e)
    {
    }
}