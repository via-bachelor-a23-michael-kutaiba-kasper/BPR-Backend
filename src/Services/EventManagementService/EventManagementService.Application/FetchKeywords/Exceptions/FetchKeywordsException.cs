namespace EventManagementService.Application.FetchKeywords.Exceptions;

public class FetchKeywordsException : Exception
{
    public FetchKeywordsException(string message, Exception exception) : base(message, exception)
    {
    }
}