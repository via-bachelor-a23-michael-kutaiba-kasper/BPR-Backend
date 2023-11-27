namespace EventManagementService.Application.FetchCategories.Exceptions;

public class FetchCategoriesException : Exception
{
    public FetchCategoriesException(string message, Exception exception) : base(message, exception)
    {
    }
}