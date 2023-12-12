namespace EventManagementService.Application.V1.FetchCategories.Exceptions;

public class NoCategoriesInDatabaseException : Exception
{
    public NoCategoriesInDatabaseException(string message) : base(message)
    {
    }
}