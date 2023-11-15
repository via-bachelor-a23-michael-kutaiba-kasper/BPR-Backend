namespace EventManagementService.Application.JoinEvent.Exceptions;

public class UserNotFoundException: Exception
{
    public UserNotFoundException(string userId, Exception? inner = null):base($"User with id {userId} does not exist in the system", inner)
    {
    }
}