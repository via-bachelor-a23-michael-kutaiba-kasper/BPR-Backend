namespace UserManagementService.Application.V1.GetUserExpProgres.Exceptions;

public class UnableToRetrieveLevelsException: Exception
{
    public UnableToRetrieveLevelsException(Exception? inner = null): base("Unable to retrieve all levels", inner)
    {
        
    }
}