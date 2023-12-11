namespace UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message)
    {
    }
}