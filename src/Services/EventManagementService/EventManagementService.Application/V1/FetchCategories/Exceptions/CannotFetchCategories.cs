namespace EventManagementService.Application.V1.FetchCategories.Exceptions;

public class CannotFetchCategories : Exception
{
    public CannotFetchCategories(string message, Exception exception) : base(message, exception)
    {
    }
}