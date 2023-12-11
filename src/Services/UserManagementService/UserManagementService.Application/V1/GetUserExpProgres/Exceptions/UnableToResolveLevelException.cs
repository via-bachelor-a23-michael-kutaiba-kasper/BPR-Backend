namespace UserManagementService.Application.V1.GetUserExpProgres.Exceptions;

public class UnableToResolveLevelException: Exception
{
    public UnableToResolveLevelException(long totalExp) : base($"Unable to ")
    {
        
    }
}