namespace EventManagementService.Application.FetchKeywords.Exceptions;

public class CannotFetchKeywords : Exception
{
    public CannotFetchKeywords(string message, Exception exception) : base(message, exception)
    {
    }
}