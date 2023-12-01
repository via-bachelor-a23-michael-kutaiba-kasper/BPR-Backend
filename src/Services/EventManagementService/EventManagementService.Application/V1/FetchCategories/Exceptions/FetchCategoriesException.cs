namespace EventManagementService.Application.V1.FetchCategories.Exceptions;

public class FetchCategoriesException : Exception
{
    public FetchCategoriesException(string message, Exception exception) : base(message, exception)
    {
    }
}