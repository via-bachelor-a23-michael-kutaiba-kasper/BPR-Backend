namespace UserManagementService.Application.V1.FetchUserAchievements.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message)
    {
    }
}