namespace UserManagementService.Application.V1.FetchUserAchievements.Exceptions;

public class FetchUserAchievementsException : Exception
{
    public FetchUserAchievementsException(string message, Exception e) : base(message, e)
    {
    }
}