namespace EventManagementService.Application.FetchKeywords.Exceptions;

public class NoKeywordsInDatabaseException : Exception
{
    public NoKeywordsInDatabaseException(string message, Exception exception) : base(message, exception)
    {
    }
    public NoKeywordsInDatabaseException(string message) : base(message)
    {
    }
}