namespace EventManagementService.Application.V1.FetchKeywords.Exceptions;

public class FetchKeywordsException : Exception
{
    public FetchKeywordsException(string message, Exception exception) : base(message, exception)
    {
    }
}