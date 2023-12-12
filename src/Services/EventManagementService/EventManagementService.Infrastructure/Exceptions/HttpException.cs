namespace EventManagementService.Infrastructure.Exceptions;

public class HttpException : Exception
{
    public HttpException(string msg) : base(msg)
    {
    }
}