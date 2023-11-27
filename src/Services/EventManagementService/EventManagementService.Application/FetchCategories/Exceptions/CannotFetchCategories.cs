namespace EventManagementService.Application.FetchCategories.Exceptions;

public class CannotFetchCategories : Exception
{
    public CannotFetchCategories(string message, Exception exception) : base(message, exception)
    {
    }
}