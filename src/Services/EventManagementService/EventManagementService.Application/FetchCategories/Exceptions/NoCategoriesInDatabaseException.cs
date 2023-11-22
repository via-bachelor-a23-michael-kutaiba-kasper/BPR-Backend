namespace EventManagementService.Application.FetchCategories.Exceptions;

public class NoCategoriesInDatabaseException : Exception
{
    public NoCategoriesInDatabaseException(string message, Exception exception) : base(message, exception)
    {
    }

    public NoCategoriesInDatabaseException(string message) : base(message)
    {
    }
}