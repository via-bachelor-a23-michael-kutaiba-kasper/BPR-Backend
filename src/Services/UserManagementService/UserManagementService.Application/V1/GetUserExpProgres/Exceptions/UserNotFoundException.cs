namespace UserManagementService.Application.V1.GetUserExpProgres.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string userId, Exception? inner = null) : base($"User with id {userId} does not exist",
        inner)

    {
    }
}