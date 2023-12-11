namespace UserManagementService.Application.V1.GetUserExpProgres.Exceptions;

public class UnableToRetrieveUserExpProgressException : Exception
{
    public UnableToRetrieveUserExpProgressException(string userId, Exception? inner = null): base($"Unable to retrieve EXP progress for user {userId}", inner)
    {
    }
}