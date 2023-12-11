namespace UserManagementService.Application.V1.FetchAllAchievements.Exceptions;

public class FetchAllAchievementsException : Exception
{
    public FetchAllAchievementsException(string message, Exception e) : base(message, e)
    {
    }
}