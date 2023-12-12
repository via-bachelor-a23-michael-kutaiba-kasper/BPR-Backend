namespace EventManagementService.Application.V1.FetchKeywords.Exceptions;

public class NoKeywordsInDatabaseException : Exception
{
    public NoKeywordsInDatabaseException(string message) : base(message)
    {
    }
}